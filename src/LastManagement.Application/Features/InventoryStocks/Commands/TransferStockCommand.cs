using LastManagement.Application.Features.InventoryStocks.Interfaces;
using LastManagement.Application.Features.Locations.Interfaces;
using LastManagement.Domain.InventoryStocks;

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
            throw new InvalidOperationException("Source and destination locations cannot be the same");

        // Validate locations exist
        var fromLocationExists = await _locationRepository.ExistsAsync(request.FromLocationId, cancellationToken);
        var toLocationExists = await _locationRepository.ExistsAsync(request.ToLocationId, cancellationToken);

        if (!fromLocationExists)
            throw new KeyNotFoundException($"Source location {request.FromLocationId} not found");
        if (!toLocationExists)
            throw new KeyNotFoundException($"Destination location {request.ToLocationId} not found");

        // Start transaction
        using var transaction = await _stockRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            // Get source stock
            var fromStock = await _stockRepository.GetByCompositeKeyAsync(
                request.LastId, request.SizeId, request.FromLocationId, cancellationToken);

            if (fromStock == null)
                throw new KeyNotFoundException(
                    $"Source stock not found (Last: {request.LastId}, Size: {request.SizeId}, Location: {request.FromLocationId})");

            if (fromStock.QuantityGood < request.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock at source. Available: {fromStock.QuantityGood}, Requested: {request.Quantity}");

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
                "Transfer",
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

            // Save all changes
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