using LastManagement.Domain.PurchaseOrders.Entities;
using LastManagement.Domain.PurchaseOrders.Enums;

namespace LastManagement.Application.Features.PurchaseOrders.Interfaces;

public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default);
    Task<PurchaseOrder?> GetByIdWithItemsAsync(int orderId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<PurchaseOrder> Items, int TotalCount, int? NextId)> GetOrdersAsync(int limit, int? after, PurchaseOrderStatus? status, string? requestedBy, CancellationToken cancellationToken = default);
    Task<IEnumerable<PurchaseOrder>> GetPendingOrdersAsync(CancellationToken cancellationToken = default);
    Task<string?> GetLastOrderNumberForDateAsync(string datePrefix, CancellationToken cancellationToken = default);
    Task<PurchaseOrder> CreateAsync(PurchaseOrder order, CancellationToken cancellationToken = default);
    Task UpdateAsync(PurchaseOrder order, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int orderId, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<PurchaseOrder?> GetByIdForUpdateAsync(int orderId, CancellationToken cancellationToken = default);
}