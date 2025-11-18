using LastManagement.Application.Features.LastNames.DTOs;
using LastManagement.Application.Features.LastNames.Interfaces;
using LastManagement.Domain.LastNames.Entities;
using LastManagement.Domain.LastNames.Enums;

namespace LastManagement.Application.Features.LastNames.Commands;

public class UpdateLastNameCommand
{
    private readonly ILastNameRepository _repository;

    public UpdateLastNameCommand(ILastNameRepository repository)
    {
        _repository = repository;
    }

    public async Task<LastName> ExecuteAsync(int lastId, UpdateLastNameRequest request, CancellationToken cancellationToken = default)
    {
        var lastName = await _repository.GetByIdAsync(lastId, cancellationToken)
            ?? throw new KeyNotFoundException($"Last name with ID {lastId} not found");

        // Update last code if provided
        if (!string.IsNullOrWhiteSpace(request.LastCode) && request.LastCode != lastName.LastCode)
        {
            if (await _repository.ExistsByCodeAsync(request.LastCode, lastId, cancellationToken))
                throw new InvalidOperationException($"Last code '{request.LastCode}' already exists");

            lastName.UpdateLastCode(request.LastCode);
        }

        // Update status if provided
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<LastNameStatus>(request.Status, true, out var newStatus))
                throw new ArgumentException($"Invalid status value: {request.Status}");

            lastName.UpdateStatus(newStatus, request.DiscontinueReason);
        }

        // Transfer customer if provided
        if (request.CustomerId.HasValue && request.CustomerId.Value != lastName.CustomerId)
        {
            lastName.TransferToCustomer(request.CustomerId.Value);
        }

        await _repository.UpdateAsync(lastName, cancellationToken);
        return lastName;
    }
}