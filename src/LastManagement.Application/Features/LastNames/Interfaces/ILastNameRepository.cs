using LastManagement.Domain.LastNames.Entities;
using LastManagement.Domain.LastNames.Enums;

namespace LastManagement.Application.Features.LastNames.Interfaces;

public interface ILastNameRepository
{
    Task<LastName?> GetByIdAsync(int lastId, CancellationToken cancellationToken = default);
    Task<LastName?> GetByIdForUpdateAsync(int lastId, CancellationToken cancellationToken = default);
    Task<(List<LastName> Items, int TotalCount)> GetPagedAsync(int limit, int? afterId, int? customerId, LastNameStatus? status, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string lastCode, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> HasModelsAsync(int lastId, CancellationToken cancellationToken = default);
    Task<bool> HasInventoryAsync(int lastId, CancellationToken cancellationToken = default);
    Task<bool> HasMovementsAsync(int lastId, CancellationToken cancellationToken = default);
    Task AddAsync(LastName lastNam, CancellationToken cancellationToken = default);
    Task UpdateAsync(LastName lastName, CancellationToken cancellationToken = default);
    Task DeleteAsync(LastName lastName, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int lastId, CancellationToken cancellationToken = default);
}