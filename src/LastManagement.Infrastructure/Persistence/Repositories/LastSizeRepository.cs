using LastManagement.Application.Features.LastSizes.Interfaces;
using LastManagement.Domain.InventoryStocks;
using LastManagement.Domain.LastSizes;
using LastManagement.Domain.LastSizes.Enums;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Infrastructure.Persistence.Repositories;

public class LastSizeRepository : ILastSizeRepository
{
    private readonly ApplicationDbContext _context;

    public LastSizeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LastSize?> GetByIdAsync(int sizeId, CancellationToken cancellationToken = default)
    {
        return await _context.LastSizesRepository
            .FirstOrDefaultAsync(ls => ls.SizeId == sizeId, cancellationToken);
    }

    public async Task<LastSize?> GetByValueAsync(decimal sizeValue, CancellationToken cancellationToken = default)
    {
        return await _context.LastSizesRepository
            .AsNoTracking()
            .FirstOrDefaultAsync(ls => ls.SizeValue == sizeValue, cancellationToken);
    }

    public async Task<(List<LastSize> Items, int TotalCount)> GetPagedAsync(
        int limit,
        int? afterId,
        SizeStatus? statusFilter,
        CancellationToken cancellationToken = default)
    {
        var query = _context.LastSizesRepository.AsNoTracking();

        // Apply status filter
        if (statusFilter.HasValue)
        {
            query = query.Where(ls => ls.Status == statusFilter.Value);
        }

        // Apply cursor pagination
        if (afterId.HasValue)
        {
            var afterSize = await _context.LastSizesRepository
                .AsNoTracking()
                .FirstOrDefaultAsync(ls => ls.SizeId == afterId.Value, cancellationToken);

            if (afterSize != null)
            {
                query = query.Where(ls => ls.SizeValue > afterSize.SizeValue ||
                    (ls.SizeValue == afterSize.SizeValue && ls.SizeId > afterId.Value));
            }
        }

        // Order by size_value (natural ordering for sizes)
        query = query.OrderBy(ls => ls.SizeValue).ThenBy(ls => ls.SizeId);

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Get page
        var items = await query
            .Take(limit)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(decimal sizeValue, CancellationToken cancellationToken = default)
    {
        return await _context.LastSizesRepository
            .AsNoTracking()
            .AnyAsync(ls => ls.SizeValue == sizeValue, cancellationToken);
    }

    public async Task<bool> HasInventoryAsync(int sizeId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<InventoryStock>().AsNoTracking().AnyAsync(s => s.SizeId == sizeId, cancellationToken);
    }

    public async Task AddAsync(LastSize lastSize, CancellationToken cancellationToken = default)
    {
        await _context.LastSizesRepository.AddAsync(lastSize, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(LastSize lastSize, CancellationToken cancellationToken = default)
    {
        _context.LastSizesRepository.Update(lastSize);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(LastSize lastSize, CancellationToken cancellationToken = default)
    {
        _context.LastSizesRepository.Remove(lastSize);
        await _context.SaveChangesAsync(cancellationToken);
    }
}