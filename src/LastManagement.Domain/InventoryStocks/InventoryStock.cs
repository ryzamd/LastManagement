using LastManagement.Application.Constants;
using LastManagement.Domain.Common;
using LastManagement.Domain.Constants;
using LastManagement.Utilities.Helpers;

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
            throw new ArgumentException(DomainValidationMessages.InventoryStock.INITIAL_QUANTITY_NEGATIVE, nameof(initialQuantity));

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

    public static InventoryStock Create(int lastId, int sizeId, int locationId, int quantityGood, int quantityDamaged, int quantityReserved)
    {
        return new InventoryStock
        {
            LastId = lastId,
            SizeId = sizeId,
            LocationId = locationId,
            QuantityGood = quantityGood,
            QuantityDamaged = quantityDamaged,
            QuantityReserved = quantityReserved,
            LastUpdated = DateTime.UtcNow,
            Version = 1
        };
    }

    public void AdjustQuantity(int quantityGoodDelta, int quantityDamagedDelta, int quantityReservedDelta)
    {
        QuantityGood += quantityGoodDelta;
        QuantityDamaged += quantityDamagedDelta;
        QuantityReserved += quantityReservedDelta;
        LastUpdated = DateTime.UtcNow;
        Version++;
    }

    public void AdjustQuantity(AdjustmentType adjustmentType, int quantity, string reason)
    {
        if (quantity <= 0)
            throw new ArgumentException(DomainValidationMessages.InventoryStock.ADJUSTMENT_QUANTITY_POSITIVE, nameof(quantity));

        switch (adjustmentType)
        {
            case AdjustmentType.Add:
                QuantityGood += quantity;
                break;

            case AdjustmentType.Remove:
                if (QuantityGood < quantity)
                    throw new InvalidOperationException(StringFormatter.FormatMessage(ErrorMessages.Inventory.INSUFFICIENT_STOCK, QuantityGood, quantity));
                QuantityGood -= quantity;
                break;

            case AdjustmentType.Damage:
                if (QuantityGood < quantity)
                    throw new InvalidOperationException(StringFormatter.FormatMessage(ErrorMessages.Inventory.CANNOT_DAMAGE, quantity, QuantityGood));
                QuantityGood -= quantity;
                QuantityDamaged += quantity;
                break;

            case AdjustmentType.Repair:
                if (QuantityDamaged < quantity)
                    throw new InvalidOperationException(StringFormatter.FormatMessage(ErrorMessages.Inventory.CANNOT_REPAIR, quantity, QuantityDamaged));
                QuantityDamaged -= quantity;
                QuantityGood += quantity;
                break;

            default:
                throw new ArgumentException(StringFormatter.FormatMessage(ErrorMessages.Inventory.UNKNOWN_ADJUSTMENT_TYPE, adjustmentType));
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
            throw new ArgumentException(DomainValidationMessages.InventoryStock.ADJUSTMENT_QUANTITY_POSITIVE, nameof(quantity));

        var available = GetAvailableQuantity();
        if (available < quantity)
            throw new InvalidOperationException(StringFormatter.FormatMessage(ErrorMessages.Inventory.CANNOT_RESERVE, quantity, available));

        QuantityReserved += quantity;
        LastUpdated = DateTime.UtcNow;
        Version++;
    }

    public void ReleaseReservation(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(DomainValidationMessages.InventoryStock.ADJUSTMENT_QUANTITY_POSITIVE, nameof(quantity));

        if (QuantityReserved < quantity)
            throw new InvalidOperationException(StringFormatter.FormatMessage(ErrorMessages.Inventory.CANNOT_RELEASE, quantity, QuantityReserved));

        QuantityReserved -= quantity;
        LastUpdated = DateTime.UtcNow;
        Version++;
    }
}