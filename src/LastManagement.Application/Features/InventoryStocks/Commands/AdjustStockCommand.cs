using LastManagement.Application.Constants;
using LastManagement.Application.Features.InventoryStocks.DTOs;
using LastManagement.Application.Features.InventoryStocks.Interfaces;
using LastManagement.Domain.Constants;
using LastManagement.Domain.InventoryStocks;
using LastManagement.Utilities.Helpers;

namespace LastManagement.Application.Features.InventoryStocks.Commands;

public class AdjustStockCommand
{
    private readonly IInventoryStockRepository _stockRepository;
    private readonly IInventoryMovementRepository _movementRepository;

    public AdjustStockCommand(
        IInventoryStockRepository stockRepository,
        IInventoryMovementRepository movementRepository)
    {
        _stockRepository = stockRepository;
        _movementRepository = movementRepository;
    }

    public async Task<AdjustStockResult> ExecuteAsync(int stockId, AdjustStockRequest request, string adminUser, CancellationToken cancellationToken = default)
    {
        var stock = await _stockRepository.GetByIdAsync(stockId, cancellationToken);
        if (stock == null)
            throw new KeyNotFoundException(StringFormatter.FormatMessage(ErrorMessages.Stock.NOT_FOUND, stockId));

        // Store previous values
        var previousGood = stock.QuantityGood;
        var previousDamaged = stock.QuantityDamaged;
        var previousVersion = stock.Version;

        // Apply adjustment
        stock.AdjustQuantity(request.AdjustmentType, request.Quantity, request.Reason);

        // Create movement log
        var movement = InventoryMovement.Create(
            stock.LastId,
            stock.SizeId,
            stock.LocationId,
            null, // No location transfer
            MapAdjustmentTypeToMovementType(request.AdjustmentType),
            request.Quantity,
            request.Reason,
            request.ReferenceNumber,
            adminUser
        );

        await _movementRepository.AddAsync(movement, cancellationToken);
        await _stockRepository.SaveChangesAsync(cancellationToken);

        return new AdjustStockResult
        {
            StockId = stock.StockId,
            AdjustmentType = request.AdjustmentType,
            PreviousQuantityGood = previousGood,
            NewQuantityGood = stock.QuantityGood,
            PreviousQuantityDamaged = previousDamaged,
            NewQuantityDamaged = stock.QuantityDamaged,
            Version = stock.Version,
            MovementId = movement.MovementId
        };
    }

    private static string MapAdjustmentTypeToMovementType(AdjustmentType type) => type switch
    {
        AdjustmentType.Add => MovementTypeConstants.ADJUSTMENT,
        AdjustmentType.Remove => MovementTypeConstants.ADJUSTMENT,
        AdjustmentType.Damage => MovementTypeConstants.DAMAGE,
        AdjustmentType.Repair => MovementTypeConstants.REPAIR,
        _ => MovementTypeConstants.ADJUSTMENT
    };
}