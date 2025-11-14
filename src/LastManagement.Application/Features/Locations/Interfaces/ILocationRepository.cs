using LastManagement.Domain.Locations;

namespace LastManagement.Application.Features.Locations.Interfaces;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Location?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Location>> GetAllAsync(string? filterType, bool? filterActive, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string locationCode, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> HasInventoryAsync(int locationId, CancellationToken cancellationToken = default);
    Task AddAsync(Location location, CancellationToken cancellationToken = default);
    void Update(Location location);
    void Delete(Location location);
}