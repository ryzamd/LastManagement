using LastManagement.Api.Constants;
using LastManagement.Api.Global.Helpers;
using LastManagement.Application.Constants;
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
            throw new ArgumentException(ErrorMessages.LastModel.LAST_ID_MUST_BE_POSITIVE_INTERGER, nameof(lastId));

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
                self = new { href = UrlHelper.FormatResourceUrl(ApiRoutes.LastModels.FULL_BY_MODEL_ID, m.LastModelId) },
                last = new { href = UrlHelper.FormatResourceUrl(ApiRoutes.LastNames.FULL_BY_ID_TEMPLATE, m.LastId) }
            }
        });
    }
}