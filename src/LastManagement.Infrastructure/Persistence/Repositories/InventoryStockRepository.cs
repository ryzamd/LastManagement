using LastManagement.Application.Features.InventoryStocks.DTOs;
using LastManagement.Application.Features.InventoryStocks.Interfaces;
using LastManagement.Domain.InventoryStocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Runtime.CompilerServices;

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
        return await _context.InventoryStocksRepository.FirstOrDefaultAsync(s => s.StockId == stockId, cancellationToken);
    }

    public async Task<InventoryStock?> GetByCompositeKeyAsync(int lastId, int sizeId, int locationId, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryStocksRepository
            .FirstOrDefaultAsync(s =>
                s.LastId == lastId &&
                s.SizeId == sizeId &&
                s.LocationId == locationId,
                cancellationToken);
    }

    public async Task<(IEnumerable<InventoryStock> Items, int TotalCount)> GetPagedAsync(int? lastId, int? sizeId, int? locationId, string? cursor, int limit, CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryStocksRepository.AsNoTracking();

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
            if (int.TryParse(decodedCursor, out var cursorId))
            {
                query = query.Where(s => s.StockId > cursorId);
            }
        }

        var items = await query
            .OrderBy(s => s.StockId)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(int lastId, int sizeId, int locationId, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryStocksRepository.AnyAsync(s => s.LastId == lastId && s.SizeId == sizeId && s.LocationId == locationId, cancellationToken);
    }

    public async Task AddAsync(InventoryStock stock, CancellationToken cancellationToken = default)
    {
        await _context.InventoryStocksRepository.AddAsync(stock, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventorySummaryRaw>> GetSummaryAsync(int? customerId, int? lastId, int? locationId, CancellationToken cancellationToken = default)
    {
        if (!await ViewExistsAsync("v_inventory_summary"))
            throw new InvalidOperationException("Database view v_inventory_summary not found. Run migrations.");

        var sql = @"SELECT 
                    v.last_id as LastId,
                    v.last_code as LastCode,
                    v.last_status as LastStatus,
                    v.customer_name as CustomerName,
                    v.location_name as LocationName,
                    v.size_label as SizeLabel,
                    v.quantity_good as QuantityGood,
                    v.quantity_damaged as QuantityDamaged,
                    v.quantity_reserved as QuantityReserved,
                    v.available_quantity as AvailableQuantity
                    FROM v_inventory_summary v
                    WHERE 1=1";

        var parameters = new List<object>();

        if (lastId.HasValue)
        {
            sql += $" AND v.last_id = {{{parameters.Count}}}";
            parameters.Add(lastId.Value);
        }

        sql += " ORDER BY v.last_code, v.size_label";

        var formattable = FormattableStringFactory.Create(sql, parameters.ToArray());
        return await _context.Database
            .SqlQuery<InventorySummaryRaw>(formattable)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<LowStockRaw> Items, int TotalCount, int CriticalCount, int WarningCount)> GetLowStockAsync(int threshold, CancellationToken cancellationToken = default)
    {
        var items = await _context.Database
            .SqlQueryRaw<LowStockRaw>(
                @"SELECT 
                    ist.stock_id as StockId,
                    l.last_code as LastCode,
                    s.size_label as SizeLabel,
                    loc.location_name as LocationName,
                    (ist.quantity_good - ist.quantity_reserved) as AvailableQuantity
                FROM inventory_stocks ist
                JOIN last_names l ON ist.last_id = l.last_id
                JOIN last_sizes s ON ist.size_id = s.size_id
                JOIN locations loc ON ist.location_id = loc.location_id
                WHERE (ist.quantity_good - ist.quantity_reserved) < {0}
                ORDER BY (ist.quantity_good - ist.quantity_reserved) ASC, l.last_code",
                threshold)
            .ToListAsync(cancellationToken);

        // Safe calculation on typed results
        var itemsList = items.ToList();
        var totalCount = itemsList.Count;
        var criticalCount = itemsList.Count(i => i.AvailableQuantity < threshold / 2);
        var warningCount = totalCount - criticalCount;

        return (itemsList, totalCount, criticalCount, warningCount);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    private async Task<bool> ViewExistsAsync(string viewName)
    {
        var result = await _context.Database
            .SqlQueryRaw<int>("SELECT 1 FROM information_schema.views WHERE table_name = {0} AND table_schema = 'public'", viewName)
            .ToListAsync();
        return result.Any();
    }
}