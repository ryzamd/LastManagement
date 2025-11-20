using LastManagement.Application.Features.PurchaseOrders.Interfaces;
using LastManagement.Domain.PurchaseOrders.Entities;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Infrastructure.Persistence.Repositories;

public sealed class PurchaseOrderItemRepository : IPurchaseOrderItemRepository
{
    private readonly ApplicationDbContext _context;

    public PurchaseOrderItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PurchaseOrderItem>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderItemsRepository
            .AsNoTracking()
            .Where(poi => poi.OrderId == orderId)
            .ToListAsync(cancellationToken);
    }

    public async Task CreateBatchAsync(IEnumerable<PurchaseOrderItem> items, CancellationToken cancellationToken = default)
    {
        await _context.PurchaseOrderItemsRepository.AddRangeAsync(items, cancellationToken);
    }
}