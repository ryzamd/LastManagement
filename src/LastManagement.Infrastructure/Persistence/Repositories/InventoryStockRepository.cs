using LastManagement.Application.Features.InventoryStocks.Interfaces;
using LastManagement.Domain.InventoryStocks;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Infrastructure.Persistence.Repositories;

public class InventoryStockRepository : IInventoryStockRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryStockRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryStock?> GetByIdAsync(int stockId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<InventoryStock>()
            .FirstOrDefaultAsync(s => s.StockId == stockId, cancellationToken);
    }

    public async Task<InventoryStock?> GetByCompositeKeyAsync(
        int lastId, int sizeId, int locationId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<InventoryStock>()
            .FirstOrDefaultAsync(s =>
                s.LastId == lastId &&
                s.SizeId == sizeId &&
                s.LocationId == locationId,
                cancellationToken);
    }

    public async Task<(IEnumerable<InventoryStock> Items, int TotalCount)> GetPagedAsync(int? lastId, int? sizeId, int? locationId, string? cursor, int limit, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<InventoryStock>()
            .AsNoTracking()
            .AsQueryable();
        // REMOVE: .Include(s => s.Last).Include(s => s.Size).Include(s => s.Location)

        if (lastId.HasValue)
            query = query.Where(s => s.LastId == lastId.Value);

        if (sizeId.HasValue)
            query = query.Where(s => s.SizeId == sizeId.Value);

        if (locationId.HasValue)
            query = query.Where(s => s.LocationId == locationId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrEmpty(cursor))
        {
            var decodedCursor = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
            if (int.TryParse(decodedCursor, out var cursorStockId))
            {
                query = query.Where(s => s.StockId > cursorStockId);
            }
        }

        var items = await query
            .OrderBy(s => s.StockId)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(int lastId, int sizeId, int locationId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<InventoryStock>()
            .AsNoTracking()
            .AnyAsync(s =>
                s.LastId == lastId &&
                s.SizeId == sizeId &&
                s.LocationId == locationId,
                cancellationToken);
    }

    public async Task AddAsync(InventoryStock stock, CancellationToken cancellationToken = default)
    {
        await _context.Set<InventoryStock>().AddAsync(stock, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}