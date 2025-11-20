using LastManagement.Domain.LastSizes;
using LastManagement.Domain.LastSizes.Enums;

namespace LastManagement.Application.Features.LastSizes.Interfaces;

public interface ILastSizeRepository
{
    Task<LastSize?> GetByIdAsync(int sizeId, CancellationToken cancellationToken = default);
    Task<LastSize?> GetByValueAsync(decimal sizeValue, CancellationToken cancellationToken = default);
    Task<(List<LastSize> Items, int TotalCount)> GetPagedAsync(int limit, int? afterId, SizeStatus? statusFilter, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(decimal sizeValue, CancellationToken cancellationToken = default);
    Task<bool> HasInventoryAsync(int sizeId, CancellationToken cancellationToken = default);
    Task AddAsync(LastSize lastSize, CancellationToken cancellationToken = default);
    Task UpdateAsync(LastSize lastSize, CancellationToken cancellationToken = default);
    Task DeleteAsync(LastSize lastSize, CancellationToken cancellationToken = default);
    Task<bool> ExistsSizeIdAsync(int sizeValue, CancellationToken cancellationToken = default);
}