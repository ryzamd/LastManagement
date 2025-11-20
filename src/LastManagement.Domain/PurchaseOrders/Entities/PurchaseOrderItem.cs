using LastManagement.Domain.Common;

namespace LastManagement.Domain.PurchaseOrders.Entities;

public sealed class PurchaseOrderItem : Entity
{
    public int ItemId { get; private set; }
    public int OrderId { get; private set; }
    public int LastId { get; private set; }
    public int SizeId { get; private set; }
    public int QuantityRequested { get; private set; }

    // Navigation property
    public PurchaseOrder Order { get; private set; } = null!;

    private PurchaseOrderItem() { } // EF Core

    public static PurchaseOrderItem Create(int orderId, int lastId, int sizeId, int quantityRequested)
    {
        if (orderId <= 0)
            throw new ArgumentException("Order ID must be positive", nameof(orderId));

        if (lastId <= 0)
            throw new ArgumentException("Last ID must be positive", nameof(lastId));

        if (sizeId <= 0)
            throw new ArgumentException("Size ID must be positive", nameof(sizeId));

        if (quantityRequested <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantityRequested));

        return new PurchaseOrderItem
        {
            OrderId = orderId,
            LastId = lastId,
            SizeId = sizeId,
            QuantityRequested = quantityRequested
        };
    }

    //public override void IncrementVersion()
    //{
    //    // Items don't have independent versioning
    //}
}