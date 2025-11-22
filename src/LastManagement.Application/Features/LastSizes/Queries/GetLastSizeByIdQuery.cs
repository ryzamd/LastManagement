using LastManagement.Api.Constants;
using LastManagement.Api.Global.Helpers;
using LastManagement.Application.Features.LastSizes.DTOs;
using LastManagement.Application.Features.LastSizes.Interfaces;

namespace LastManagement.Application.Features.LastSizes.Queries;

public class GetLastSizeByIdQuery
{
    private readonly ILastSizeRepository _repository;

    public GetLastSizeByIdQuery(ILastSizeRepository repository)
    {
        _repository = repository;
    }

    public async Task<LastSizeDto?> ExecuteAsync(int sizeId, CancellationToken cancellationToken = default)
    {
        var lastSize = await _repository.GetByIdAsync(sizeId, cancellationToken);

        return lastSize == null ? null : MapToDto(lastSize);
    }

    private static LastSizeDto MapToDto(Domain.LastSizes.LastSize size)
    {
        return new LastSizeDto
        {
            Id = size.SizeId,
            SizeValue = size.SizeValue,
            SizeLabel = size.SizeLabel,
            Status = size.Status.ToString(),
            ReplacementSizeId = size.ReplacementSizeId,
            CreatedAt = size.CreatedAt,
            UpdatedAt = size.UpdatedAt,
            Links = new Dictionary<string, object>
            {
                ["self"] = new { href = UrlHelper.FormatResourceUrl(ApiRoutes.LastSizes.FULL_BY_ID_TEMPLATE, size.SizeId) }
            }
        };
    }
}