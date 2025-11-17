using LastManagement.Domain.Common;

namespace LastManagement.Domain.InventoryStocks;

public class InventoryStock : Entity
{
    public int StockId { get; private set; }
    public int LastId { get; private set; }
    public int SizeId { get; private set; }
    public int LocationId { get; private set; }
    public int QuantityGood { get; private set; }
    public int QuantityDamaged { get; private set; }
    public int QuantityReserved { get; private set; }
    public DateTime LastUpdated { get; private set; }
    public new int Version { get; private set; }

    private InventoryStock() { }

    public static InventoryStock Create(int lastId, int sizeId, int locationId, int initialQuantity)
    {
        if (initialQuantity < 0)
            throw new ArgumentException("Initial quantity cannot be negative", nameof(initialQuantity));

        var stock = new InventoryStock
        {
            LastId = lastId,
            SizeId = sizeId,
            LocationId = locationId,
            QuantityGood = initialQuantity,
            QuantityDamaged = 0,
            QuantityReserved = 0,
            LastUpdated = DateTime.UtcNow,
            Version = 1
        };

        return stock;
    }

    public void AdjustQuantity(AdjustmentType adjustmentType, int quantity, string reason)
    {
        if (quantity <= 0)
            throw new ArgumentException("Adjustment quantity must be positive", nameof(quantity));

        switch (adjustmentType)
        {
            case AdjustmentType.Add:
                QuantityGood += quantity;
                break;

            case AdjustmentType.Remove:
                if (QuantityGood < quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock. Available: {QuantityGood}, Requested: {quantity}");
                QuantityGood -= quantity;
                break;

            case AdjustmentType.Damage:
                if (QuantityGood < quantity)
                    throw new InvalidOperationException(
                        $"Cannot damage {quantity} units. Only {QuantityGood} good units available.");
                QuantityGood -= quantity;
                QuantityDamaged += quantity;
                break;

            case AdjustmentType.Repair:
                if (QuantityDamaged < quantity)
                    throw new InvalidOperationException(
                        $"Cannot repair {quantity} units. Only {QuantityDamaged} damaged units available.");
                QuantityDamaged -= quantity;
                QuantityGood += quantity;
                break;

            default:
                throw new ArgumentException($"Unknown adjustment type: {adjustmentType}");
        }

        LastUpdated = DateTime.UtcNow;
        Version++;

        AddDomainEvent(new StockAdjustedEvent(StockId, LastId, SizeId, LocationId,
            adjustmentType, quantity, reason));
    }

    public int GetAvailableQuantity()
    {
        return QuantityGood - QuantityReserved;
    }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Reserve quantity must be positive", nameof(quantity));

        var available = GetAvailableQuantity();
        if (available < quantity)
            throw new InvalidOperationException(
                $"Cannot reserve {quantity} units. Only {available} available.");

        QuantityReserved += quantity;
        LastUpdated = DateTime.UtcNow;
        Version++;
    }

    public void ReleaseReservation(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Release quantity must be positive", nameof(quantity));

        if (QuantityReserved < quantity)
            throw new InvalidOperationException(
                $"Cannot release {quantity} units. Only {QuantityReserved} reserved.");

        QuantityReserved -= quantity;
        LastUpdated = DateTime.UtcNow;
        Version++;
    }
}