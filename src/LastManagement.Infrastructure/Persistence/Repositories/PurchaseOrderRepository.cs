using LastManagement.Application.Features.PurchaseOrders.Interfaces;
using LastManagement.Domain.PurchaseOrders.Entities;
using LastManagement.Domain.PurchaseOrders.Enums;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Infrastructure.Persistence.Repositories;

public sealed class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly ApplicationDbContext _context;

    public PurchaseOrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrder?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrdersRepository
            .AsNoTracking()
            .FirstOrDefaultAsync(po => po.OrderId == orderId, cancellationToken);
    }

    public async Task<PurchaseOrder?> GetByIdWithItemsAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrdersRepository
            .Include(po => po.Items)
            .FirstOrDefaultAsync(po => po.OrderId == orderId, cancellationToken);
    }

    public async Task<(IEnumerable<PurchaseOrder> Items, int TotalCount, int? NextId)> GetOrdersAsync(int limit, int? after, PurchaseOrderStatus? status, string? requestedBy, CancellationToken cancellationToken = default)
    {
        var query = _context.PurchaseOrdersRepository.AsNoTracking();

        if (after.HasValue)
        {
            query = query.Where(po => po.OrderId > after.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(po => po.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(requestedBy))
        {
            query = query.Where(po => po.RequestedBy == requestedBy);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(po => po.OrderId)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var nextId = items.Count == limit ? items.Last().OrderId : (int?)null;

        return (items, totalCount, nextId);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetPendingOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrdersRepository
            .AsNoTracking()
            .Where(po => po.Status == PurchaseOrderStatus.Pending)
            .OrderBy(po => po.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<string?> GetLastOrderNumberForDateAsync(string datePrefix, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrdersRepository
            .AsNoTracking()
            .Where(po => po.OrderNumber.StartsWith($"PO-{datePrefix}"))
            .OrderByDescending(po => po.OrderNumber)
            .Select(po => po.OrderNumber)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PurchaseOrder> CreateAsync(PurchaseOrder order, CancellationToken cancellationToken = default)
    {
        await _context.PurchaseOrdersRepository.AddAsync(order, cancellationToken);
        return order;
    }

    public async Task UpdateAsync(PurchaseOrder order, CancellationToken cancellationToken = default)
    {
        _context.PurchaseOrdersRepository.Update(order);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrdersRepository
            .AsNoTracking()
            .AnyAsync(po => po.OrderId == orderId, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PurchaseOrder?> GetByIdForUpdateAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrdersRepository.FirstOrDefaultAsync(po => po.OrderId == orderId, cancellationToken);
    }
}