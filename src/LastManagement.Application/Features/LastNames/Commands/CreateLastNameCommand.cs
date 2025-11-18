using LastManagement.Application.Features.LastNames.DTOs;
using LastManagement.Application.Features.LastNames.Interfaces;
using LastManagement.Domain.LastNames.Entities;

namespace LastManagement.Application.Features.LastNames.Commands;

public class CreateLastNameCommand
{
    private readonly ILastNameRepository _repository;

    public CreateLastNameCommand(ILastNameRepository repository)
    {
        _repository = repository;
    }

    public async Task<LastName> ExecuteAsync(CreateLastNameRequest request, CancellationToken cancellationToken = default)
    {
        // Check duplicate last_code
        if (await _repository.ExistsByCodeAsync(request.LastCode, null, cancellationToken))
            throw new InvalidOperationException($"Last code '{request.LastCode}' already exists");

        var lastName = LastName.Create(request.CustomerId, request.LastCode);
        await _repository.AddAsync(lastName, cancellationToken);

        return lastName;
    }
}