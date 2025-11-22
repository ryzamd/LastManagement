using LastManagement.Domain.Common;
using LastManagement.Domain.Constants;

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
            throw new ArgumentException(DomainValidationMessages.PurchaseOrderItem.ORDER_ID_POSITIVE, nameof(orderId));

        if (lastId <= 0)
            throw new ArgumentException(DomainValidationMessages.PurchaseOrderItem.LAST_ID_POSITIVE, nameof(lastId));

        if (sizeId <= 0)
            throw new ArgumentException(DomainValidationMessages.PurchaseOrderItem.SIZE_ID_POSITIVE, nameof(sizeId));

        if (quantityRequested <= 0)
            throw new ArgumentException(DomainValidationMessages.PurchaseOrderItem.QUANTITY_POSITIVE, nameof(quantityRequested));

        return new PurchaseOrderItem
        {
            OrderId = orderId,
            LastId = lastId,
            SizeId = sizeId,
            QuantityRequested = quantityRequested,
            CreatedAt = DateTime.UtcNow
        };
    }
}