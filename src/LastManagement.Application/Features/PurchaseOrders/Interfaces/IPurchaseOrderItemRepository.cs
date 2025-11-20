using LastManagement.Domain.PurchaseOrders.Entities;

namespace LastManagement.Application.Features.PurchaseOrders.Interfaces;

public interface IPurchaseOrderItemRepository
{
    Task<IEnumerable<PurchaseOrderItem>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
    Task CreateBatchAsync(IEnumerable<PurchaseOrderItem> items, CancellationToken cancellationToken = default);
}