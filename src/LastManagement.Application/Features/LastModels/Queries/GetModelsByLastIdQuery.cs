using LastManagement.Application.Features.LastModels.DTOs;
using LastManagement.Application.Features.LastModels.Interfaces;

namespace LastManagement.Application.Features.LastModels.Queries;

public class GetModelsByLastIdQuery
{
    private readonly ILastModelRepository _repository;

    public GetModelsByLastIdQuery(ILastModelRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LastModelDto>> ExecuteAsync(int lastId, CancellationToken cancellationToken = default)
    {
        if (lastId <= 0)
            throw new ArgumentException("Last ID must be positive", nameof(lastId));

        var models = await _repository.GetByLastIdAsync(lastId, cancellationToken);

        return models.Select(m => new LastModelDto
        {
            Id = m.LastModelId,
            LastId = m.LastId,
            ModelCode = m.ModelCode,
            Status = m.Status.ToString(),
            CreatedAt = m.CreatedAt,
            Links = new
            {
                self = new { href = $"/api/v1/last-models/{m.LastModelId}" },
                last = new { href = $"/api/v1/last-names/{m.LastId}" }
            }
        });
    }
}