using LastManagement.Domain.Common;

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

    public static InventoryMovement Create(
        int lastId,
        int sizeId,
        int? fromLocationId,
        int? toLocationId,
        string movementType,
        int quantity,
        string? reason,
        string? referenceNumber,
        string? createdBy)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

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
}