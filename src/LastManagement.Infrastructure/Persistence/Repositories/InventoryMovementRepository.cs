using LastManagement.Application.Features.InventoryStocks.Interfaces;
using LastManagement.Domain.InventoryStocks;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Infrastructure.Persistence.Repositories;

public class InventoryMovementRepository : IInventoryMovementRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryMovementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryMovement?> GetByIdAsync(int movementId, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryMovementsRepository
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.MovementId == movementId, cancellationToken);
    }

    public async Task<(IEnumerable<InventoryMovement> Items, int TotalCount)> GetPagedAsync(int? lastId, string? movementType, DateTime? fromDate, DateTime? toDate, string? cursor, int limit, CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryMovementsRepository.AsNoTracking();

        if (lastId.HasValue)
            query = query.Where(m => m.LastId == lastId.Value);

        if (!string.IsNullOrEmpty(movementType))
            query = query.Where(m => m.MovementType == movementType);

        if (fromDate.HasValue)
            query = query.Where(m => m.CreatedAt >= fromDate.Value.ToUniversalTime());

        if (toDate.HasValue)
            query = query.Where(m => m.CreatedAt <= toDate.Value.ToUniversalTime());

        var totalCount = await query.CountAsync(cancellationToken);

        // Cursor by MovementId to match ordering
        if (!string.IsNullOrEmpty(cursor))
        {
            var decodedCursor = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
            if (int.TryParse(decodedCursor, out var cursorId))
            {
                query = query.Where(m => m.MovementId < cursorId);  // DESC order
            }
        }

        var items = await query
            .OrderByDescending(m => m.MovementId)  // Consistent with cursor
            .Take(limit)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(InventoryMovement movement, CancellationToken cancellationToken = default)
    {
        await _context.InventoryMovementsRepository.AddAsync(movement, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Dictionary<int, string>> GetLastNamesMapAsync(IEnumerable<int> lastIds, CancellationToken cancellationToken = default)
    {
        return await _context.LastNameRepository
            .AsNoTracking()
            .Where(l => lastIds.Contains(l.LastId))
            .ToDictionaryAsync(l => l.LastId, l => l.LastCode, cancellationToken);
    }

    public async Task<Dictionary<int, string>> GetSizesMapAsync(IEnumerable<int> sizeIds, CancellationToken cancellationToken = default)
    {
        return await _context.LastSizesRepository
            .AsNoTracking()
            .Where(s => sizeIds.Contains(s.SizeId))
            .ToDictionaryAsync(s => s.SizeId, s => s.SizeLabel, cancellationToken);
    }

    public async Task<Dictionary<int, string>> GetLocationsMapAsync(IEnumerable<int> locationIds, CancellationToken cancellationToken = default)
    {
        return await _context.LocationsRepository
            .AsNoTracking()
            .Where(loc => locationIds.Contains(loc.Id))
            .ToDictionaryAsync(loc => loc.Id, loc => loc.LocationName, cancellationToken);
    }

    public async Task CreateAsync(InventoryMovement movement, CancellationToken cancellationToken = default)
    {
        await _context.InventoryMovementsRepository.AddAsync(movement, cancellationToken);
    }
}