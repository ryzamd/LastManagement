using LastManagement.Application.Features.LastNames.DTOs;
using LastManagement.Application.Features.LastNames.Interfaces;
using LastManagement.Domain.LastNames.Enums;

namespace LastManagement.Application.Features.LastNames.Queries;

public class GetLastNamesQuery
{
    private readonly ILastNameRepository _repository;

    public GetLastNamesQuery(ILastNameRepository repository)
    {
        _repository = repository;
    }

    public async Task<(List<LastNameDto> Items, int TotalCount, int? NextId)> ExecuteAsync(
        int limit,
        int? afterId,
        int? customerId,
        string? statusFilter,
        CancellationToken cancellationToken = default)
    {
        LastNameStatus? status = null;
        if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            if (!Enum.TryParse<LastNameStatus>(statusFilter, true, out var parsedStatus))
                throw new ArgumentException($"Invalid status value: {statusFilter}");
            status = parsedStatus;
        }

        var (items, totalCount) = await _repository.GetPagedAsync(limit, afterId, customerId, status, cancellationToken);

        var dtos = items.Select(ln => new LastNameDto
        {
            Id = ln.LastId,
            LastCode = ln.LastCode,
            CustomerId = ln.CustomerId,
            Status = ln.Status.ToString(),
            DiscontinueReason = ln.DiscontinueReason,
            CreatedAt = ln.CreatedAt,
            UpdatedAt = ln.UpdatedAt
        }).ToList();

        int? nextId = items.Count == limit ? items.Last().LastId : null;

        return (dtos, totalCount, nextId);
    }
}