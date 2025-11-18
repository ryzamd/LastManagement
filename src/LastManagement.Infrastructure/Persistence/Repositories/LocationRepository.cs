using LastManagement.Application.Features.Locations.Interfaces;
using LastManagement.Domain.InventoryStocks;
using LastManagement.Domain.Locations;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Infrastructure.Persistence.Repositories;

public sealed class LocationRepository : ILocationRepository
{
    private readonly ApplicationDbContext _context;

    public LocationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Location?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.LocationsRepository.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<Location?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.LocationsRepository.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<List<Location>> GetAllAsync(string? filterType, bool? filterActive, CancellationToken cancellationToken = default)
    {
        var query = _context.LocationsRepository.AsNoTracking();

        if (!string.IsNullOrEmpty(filterType) && Enum.TryParse<LocationType>(filterType, out var type))
        {
            query = query.Where(l => l.LocationType == type);
        }

        if (filterActive.HasValue)
        {
            query = query.Where(l => l.IsActive == filterActive.Value);
        }

        return await query.OrderBy(l => l.LocationCode).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string locationCode, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.LocationsRepository.AsNoTracking().Where(l => l.LocationCode == locationCode);

        if (excludeId.HasValue)
        {
            query = query.Where(l => l.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasInventoryAsync(int locationId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<InventoryStock>().AsNoTracking().AnyAsync(s => s.LocationId == locationId, cancellationToken);
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken = default)
    {
        await _context.LocationsRepository.AddAsync(location, cancellationToken);
    }

    public void Update(Location location)
    {
        _context.LocationsRepository.Update(location);
    }

    public void Delete(Location location)
    {
        _context.LocationsRepository.Remove(location);
    }
}