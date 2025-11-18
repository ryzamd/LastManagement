using LastManagement.Domain.LastModels.Entities;

namespace LastManagement.Application.Features.LastModels.Interfaces;

public interface ILastModelRepository
{
    Task<IEnumerable<LastModel>> GetAllAsync(string? statusFilter = null, string? orderBy = null, CancellationToken cancellationToken = default);

    Task<IEnumerable<LastModel>> GetByLastIdAsync(int lastId, CancellationToken cancellationToken = default);

    Task<LastModel?> GetByIdAsync(int lastModelId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCompositeKeyAsync(int lastId, string modelCode, CancellationToken cancellationToken = default);
}