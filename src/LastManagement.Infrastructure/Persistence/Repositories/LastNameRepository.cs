using LastManagement.Application.Features.LastNames.Interfaces;
using LastManagement.Domain.LastNames.Entities;
using LastManagement.Domain.LastNames.Enums;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Infrastructure.Persistence.Repositories;

public class LastNameRepository : ILastNameRepository
{
    private readonly ApplicationDbContext _context;

    public LastNameRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LastName?> GetByIdAsync(int lastId, CancellationToken cancellationToken = default)
    {
        return await _context.LastNameRepository.AsNoTracking().FirstOrDefaultAsync(ln => ln.LastId == lastId, cancellationToken);
    }

    public async Task<LastName?> GetByIdForUpdateAsync(int lastId, CancellationToken cancellationToken = default)
    {
        return await _context.LastNameRepository.FirstOrDefaultAsync(ln => ln.LastId == lastId, cancellationToken);
    }

    public async Task<(List<LastName> Items, int TotalCount)> GetPagedAsync(
        int limit,
        int? afterId,
        int? customerId,
        LastNameStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = _context.LastNameRepository.AsNoTracking();

        // Apply filters
        if (customerId.HasValue)
        {
            query = query.Where(ln => ln.CustomerId == customerId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(ln => ln.Status == status.Value);
        }

        // Apply cursor pagination
        if (afterId.HasValue)
        {
            query = query.Where(ln => ln.LastId > afterId.Value);
        }

        // Order by ID (primary key)
        query = query.OrderBy(ln => ln.LastId);

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Get page
        var items = await query
            .Take(limit)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsByCodeAsync(string lastCode, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.LastNameRepository.AsNoTracking();

        query = query.Where(ln => ln.LastCode == lastCode);

        if (excludeId.HasValue)
        {
            query = query.Where(ln => ln.LastId != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasModelsAsync(int lastId, CancellationToken cancellationToken = default)
    {
        return await _context.LastModelsRepository.AsNoTracking().AnyAsync(lm => lm.LastId == lastId, cancellationToken);
    }

    public async Task<bool> HasInventoryAsync(int lastId, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryStocksRepository.AsNoTracking().AnyAsync(s => s.LastId == lastId, cancellationToken);
    }

    public async Task<bool> HasMovementsAsync(int lastId, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryMovementsRepository.AsNoTracking().AnyAsync(m => m.LastId == lastId, cancellationToken);
    }

    public async Task AddAsync(LastName lastName, CancellationToken cancellationToken = default)
    {
        await _context.LastNameRepository.AddAsync(lastName, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(LastName lastName, CancellationToken cancellationToken = default)
    {
        _context.LastNameRepository.Update(lastName);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(LastName lastName, CancellationToken cancellationToken = default)
    {
        _context.LastNameRepository.Remove(lastName);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int lastId, CancellationToken cancellationToken = default)
    {
        return await _context.LastNameRepository.AsNoTracking().AnyAsync(ln => ln.LastId == lastId, cancellationToken);
    }
}