using LastManagement.Api.Constants;
using LastManagement.Api.Global.Helpers;
using LastManagement.Application.Features.LastModels.DTOs;
using LastManagement.Application.Features.LastModels.Interfaces;

namespace LastManagement.Application.Features.LastModels.Queries;

public class GetLastModelsQuery
{
    private readonly ILastModelRepository _repository;

    public GetLastModelsQuery(ILastModelRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LastModelDto>> ExecuteAsync(string? statusFilter = null, string? orderBy = null, CancellationToken cancellationToken = default)
    {
        var models = await _repository.GetAllAsync(statusFilter, orderBy, cancellationToken);

        return models.Select(m => new LastModelDto
        {
            Id = m.LastModelId,
            LastId = m.LastId,
            ModelCode = m.ModelCode,
            Status = m.Status.ToString(),
            CreatedAt = m.CreatedAt,
            Links = new
            {
                self = new { href = UrlHelper.FormatResourceUrl(ApiRoutes.LastModels.FULL_BY_MODEL_ID, m.LastModelId) },
                last = new { href = UrlHelper.FormatResourceUrl(ApiRoutes.LastNames.FULL_BY_ID_TEMPLATE, m.LastId) }
            }
        });
    }
}