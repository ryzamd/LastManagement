using LastManagement.Application.Features.LastSizes.DTOs;
using LastManagement.Application.Features.LastSizes.Interfaces;
using LastManagement.Domain.LastSizes;
using LastManagement.Domain.LastSizes.Enums;

namespace LastManagement.Application.Features.LastSizes.Queries;

public class GetLastSizesQuery
{
    private readonly ILastSizeRepository _repository;

    public GetLastSizesQuery(ILastSizeRepository repository)
    {
        _repository = repository;
    }

    public async Task<(List<LastSizeDto> Items, int TotalCount, int? NextId)> ExecuteAsync(
        int limit = 20,
        int? afterId = null,
        string? statusFilter = null,
        CancellationToken cancellationToken = default)
    {
        SizeStatus? status = null;
        if (!string.IsNullOrWhiteSpace(statusFilter) && Enum.TryParse<SizeStatus>(statusFilter, true, out var parsedStatus))
        {
            status = parsedStatus;
        }

        var (items, totalCount) = await _repository.GetPagedAsync(limit, afterId, status, cancellationToken);

        var dtos = items.Select(MapToDto).ToList();
        int? nextId = items.Count == limit ? items[^1].SizeId : null;

        return (dtos, totalCount, nextId);
    }

    private static LastSizeDto MapToDto(LastSize size)
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
                ["self"] = new { href = $"/api/v1/last-sizes/{size.SizeId}" }
            }
        };
    }
}