using LastManagement.Application.Constants;
using LastManagement.Application.Features.InventoryStocks.Interfaces;
using LastManagement.Application.Features.Locations.Interfaces;
using LastManagement.Domain.Constants;
using LastManagement.Domain.InventoryStocks;
using LastManagement.Utilities.Constants.Global;
using LastManagement.Utilities.Helpers;

namespace LastManagement.Application.Features.InventoryStocks.Commands;

public class TransferStockCommand
{
    private readonly IInventoryStockRepository _stockRepository;
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly ILocationRepository _locationRepository;

    public TransferStockCommand(
        IInventoryStockRepository stockRepository,
        IInventoryMovementRepository movementRepository,
        ILocationRepository locationRepository)
    {
        _stockRepository = stockRepository;
        _movementRepository = movementRepository;
        _locationRepository = locationRepository;
    }

    public async Task<TransferStockResult> ExecuteAsync(TransferStockRequest request, string adminUser, CancellationToken cancellationToken = default)
    {
        if (request.FromLocationId == request.ToLocationId)
            throw new InvalidOperationException(ResultMessages.InventoryStock.SAME_SOURCE_DESTINATION);

        // Validate locations exist
        var fromLocationExists = await _locationRepository.ExistsAsync(request.FromLocationId, cancellationToken);
        var toLocationExists = await _locationRepository.ExistsAsync(request.ToLocationId, cancellationToken);

        if (!fromLocationExists)
            throw new KeyNotFoundException(StringFormatter.FormatMessage(ErrorMessages.Stock.SOURCE_LOCATION_NOT_FOUND, request.FromLocationId));

        if (!toLocationExists)
            throw new KeyNotFoundException(StringFormatter.FormatMessage(ErrorMessages.Stock.DESTINATION_LOCATION_NOT_FOUND, request.ToLocationId));

        // Start transaction
        using var transaction = await _stockRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            // Get source stock
            var fromStock = await _stockRepository.GetByCompositeKeyAsync(
                request.LastId, request.SizeId, request.FromLocationId, cancellationToken);

            if (fromStock == null)
                throw new InvalidOperationException(StringFormatter.FormatMessage(ErrorMessages.Stock.SOURCE_NOT_FOUND, request.LastId, request.SizeId, request.FromLocationId));

            if (fromStock.QuantityGood < request.Quantity)
                throw new InvalidOperationException(StringFormatter.FormatMessage(ErrorMessages.Stock.INSUFFICIENT_AT_SOURCE, fromStock.QuantityGood, request.Quantity));

            var fromPreviousQuantity = fromStock.QuantityGood;

            // Decrease from source
            fromStock.AdjustQuantity(AdjustmentType.Remove, request.Quantity, request.Reason);

            // Get or create destination stock
            var toStock = await _stockRepository.GetByCompositeKeyAsync(
                request.LastId, request.SizeId, request.ToLocationId, cancellationToken);

            int toPreviousQuantity;
            if (toStock == null)
            {
                toStock = InventoryStock.Create(request.LastId, request.SizeId, request.ToLocationId, 0);
                await _stockRepository.AddAsync(toStock, cancellationToken);
                toPreviousQuantity = 0;
            }
            else
            {
                toPreviousQuantity = toStock.QuantityGood;
            }

            // Increase at destination
            toStock.AdjustQuantity(AdjustmentType.Add, request.Quantity, request.Reason);

            // Create movement log
            var movement = InventoryMovement.Create(
                request.LastId,
                request.SizeId,
                request.FromLocationId,
                request.ToLocationId,
                MovementTypeConstants.TRANSFER,
                request.Quantity,
                request.Reason,
                request.ReferenceNumber,
                adminUser
            );

            await _movementRepository.AddAsync(movement, cancellationToken);

            // Raise domain event BEFORE save
            fromStock.AddDomainEvent(new StockTransferredEvent(
                request.LastId, request.SizeId,
                request.FromLocationId, request.ToLocationId,
                request.Quantity, request.Reason));

            await _stockRepository.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new TransferStockResult
            {
                TransferId = Guid.NewGuid().ToString(),
                FromStock = new StockSnapshot
                {
                    Id = fromStock.StockId,
                    LocationId = request.FromLocationId,
                    PreviousQuantity = fromPreviousQuantity,
                    NewQuantity = fromStock.QuantityGood,
                    Version = fromStock.Version
                },
                ToStock = new StockSnapshot
                {
                    Id = toStock.StockId,
                    LocationId = request.ToLocationId,
                    PreviousQuantity = toPreviousQuantity,
                    NewQuantity = toStock.QuantityGood,
                    Version = toStock.Version
                },
                MovementId = movement.MovementId
            };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}