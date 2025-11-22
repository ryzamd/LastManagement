using LastManagement.Application.Constants;
using LastManagement.Application.Features.LastNames.DTOs;
using LastManagement.Application.Features.LastNames.Interfaces;
using LastManagement.Domain.LastNames.Entities;
using LastManagement.Domain.LastNames.Enums;
using LastManagement.Utilities.Constants.Global;
using LastManagement.Utilities.Helpers;

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
            ?? throw new KeyNotFoundException(StringFormatter.FormatMessage(ErrorMessages.LastName.NOT_FOUND, lastId));


        // Update last code if provided
        if (!string.IsNullOrWhiteSpace(request.LastCode) && request.LastCode != lastName.LastCode)
        {
            if (await _repository.ExistsByCodeAsync(request.LastCode, lastId, cancellationToken))
                throw new InvalidOperationException(StringFormatter.FormatMessage(ErrorMessages.LastName.ALREADY_EXISTS, request.LastCode, RoleConstants.CUSTOMER));

            lastName.UpdateLastCode(request.LastCode);
        }

        // Update status if provided
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<LastNameStatus>(request.Status, true, out var newStatus))
                throw new ArgumentException(StringFormatter.FormatMessage(ErrorMessages.LastName.INVALID_STATUS, request.Status));

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