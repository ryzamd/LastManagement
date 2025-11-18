using LastManagement.Application.Features.LastNames.DTOs;
using LastManagement.Application.Features.LastNames.Interfaces;

namespace LastManagement.Application.Features.LastNames.Queries;

public class GetLastNameByIdQuery
{
    private readonly ILastNameRepository _repository;

    public GetLastNameByIdQuery(ILastNameRepository repository)
    {
        _repository = repository;
    }

    public async Task<LastNameDto?> ExecuteAsync(int lastId, CancellationToken cancellationToken = default)
    {
        var lastName = await _repository.GetByIdAsync(lastId, cancellationToken);

        if (lastName == null)
            return null;

        return new LastNameDto
        {
            Id = lastName.LastId,
            LastCode = lastName.LastCode,
            CustomerId = lastName.CustomerId,
            Status = lastName.Status.ToString(),
            DiscontinueReason = lastName.DiscontinueReason,
            CreatedAt = lastName.CreatedAt,
            UpdatedAt = lastName.UpdatedAt
        };
    }
}