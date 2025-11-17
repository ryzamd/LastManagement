using LastManagement.Application.Features.InventoryStocks.Interfaces;
using LastManagement.Domain.InventoryStocks;

namespace LastManagement.Application.Features.InventoryStocks.Commands;

public class TransferStockCommand
{
    private readonly IInventoryStockRepository _stockRepository;
    private readonly IInventoryMovementRepository _movementRepository;

    public TransferStockCommand(
        IInventoryStockRepository stockRepository,
        IInventoryMovementRepository movementRepository)
    {
        _stockRepository = stockRepository;
        _movementRepository = movementRepository;
    }

    public async Task<TransferStockResult> ExecuteAsync(
        TransferStockRequest request,
        string adminUser,
        CancellationToken cancellationToken = default)
    {
        if (request.FromLocationId == request.ToLocationId)
            throw new InvalidOperationException("Source and destination locations cannot be the same");

        // Get source stock
        var fromStock = await _stockRepository.GetByCompositeKeyAsync(request.LastId, request.SizeId, request.FromLocationId, cancellationToken);

        if (fromStock == null)
            throw new KeyNotFoundException($"Source stock not found (Last: {request.LastId}, Size: {request.SizeId}, Location: {request.FromLocationId})");

        // Validate sufficient quantity
        if (fromStock.QuantityGood < request.Quantity)
            throw new InvalidOperationException($"Insufficient stock at source. Available: {fromStock.QuantityGood}, Requested: {request.Quantity}");

        // Decrease from source
        fromStock.AdjustQuantity(AdjustmentType.Remove, request.Quantity, request.Reason);
        var fromPreviousVersion = fromStock.Version - 1;

        // Get or create destination stock
        var toStock = await _stockRepository.GetByCompositeKeyAsync(request.LastId, request.SizeId, request.ToLocationId, cancellationToken);

        int toPreviousQuantity;
        int toPreviousVersion;

        if (toStock == null)
        {
            toStock = InventoryStock.Create(request.LastId, request.SizeId, request.ToLocationId, 0);
            await _stockRepository.AddAsync(toStock, cancellationToken);
            toPreviousQuantity = 0;
            toPreviousVersion = 0;
        }
        else
        {
            toPreviousQuantity = toStock.QuantityGood;
            toPreviousVersion = toStock.Version;
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
        await _stockRepository.SaveChangesAsync(cancellationToken);

        fromStock.AddDomainEvent(new StockTransferredEvent(
            request.LastId, request.SizeId,
            request.FromLocationId, request.ToLocationId,
            request.Quantity, request.Reason));

        return new TransferStockResult
        {
            TransferId = Guid.NewGuid().ToString(),
            FromStock = new StockSnapshot
            {
                Id = fromStock.StockId,
                LocationId = request.FromLocationId,
                PreviousQuantity = toPreviousQuantity + request.Quantity,
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
}