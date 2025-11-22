using LastManagement.Domain.Common;
using LastManagement.Domain.Constants;

namespace LastManagement.Domain.InventoryStocks;

public class InventoryMovement : Entity
{
    public int MovementId { get; private set; }
    public int LastId { get; private set; }
    public int SizeId { get; private set; }
    public int? FromLocationId { get; private set; }
    public int? ToLocationId { get; private set; }
    public string MovementType { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public string? Reason { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public new DateTime CreatedAt { get; private set; }
    public string? CreatedBy { get; private set; }

    private InventoryMovement() { } // EF Core

    public static InventoryMovement Create(int lastId, int sizeId, int? fromLocationId, int? toLocationId, string movementType, int quantity, string? reason, string? referenceNumber, string? createdBy)
    {
        if (quantity <= 0)
            throw new ArgumentException(DomainValidationMessages.InventoryStock.QUANTITY_POSITIVE, nameof(quantity));

        return new InventoryMovement
        {
            LastId = lastId,
            SizeId = sizeId,
            FromLocationId = fromLocationId,
            ToLocationId = toLocationId,
            MovementType = movementType,
            Quantity = quantity,
            Reason = reason,
            ReferenceNumber = referenceNumber,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public static InventoryMovement CreatePurchase(int lastId, int sizeId, int toLocationId, int quantity, string referenceNumber, string createdBy)
    {
        return Create(
            lastId,
            sizeId,
            fromLocationId: null,
            toLocationId: toLocationId,
            movementType: MovementTypeConstants.PURCHASE,
            quantity: quantity,
            reason: MovementReasonConstants.PURCHASE_ORDER,
            referenceNumber: referenceNumber,
            createdBy: createdBy
        );
    }
}