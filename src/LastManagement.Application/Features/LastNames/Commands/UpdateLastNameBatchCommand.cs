using LastManagement.Api.Constants;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.LastNames.DTOs;
using LastManagement.Application.Features.LastNames.Interfaces;
using LastManagement.Domain.LastNames.Enums;
using LastManagement.Utilities.Constants.Global;
using LastManagement.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;

namespace LastManagement.Application.Features.LastNames.Commands;

public class UpdateLastNameBatchCommand
{
    private readonly ILastNameRepository _repository;

    public UpdateLastNameBatchCommand(ILastNameRepository repository)
    {
        _repository = repository;
    }

    public async Task<LastNamesBatchOperationResult> ExecuteAsync(UpdateLastNameBatchRequest request, CancellationToken cancellationToken = default)
    {
        var result = new LastNamesBatchOperationResult();

        foreach (var operation in request.Operations)
        {
            try
            {
                // Use tracked entity for updates
                var lastName = await _repository.GetByIdForUpdateAsync(operation.Id, cancellationToken);

                if (lastName == null)
                {
                    result.Failed++;
                    result.Results.Add(new LastNamesBatchItemResult
                    {
                        Id = operation.Id,
                        Status = StatusContants.ERROR,
                        Error = new LastNamesBatchError
                        {
                            Type = ProblemDetailsConstants.Types.RFC_NOT_FOUND,
                            Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                            Status = 404,
                            Detail = StringFormatter.FormatMessage(ErrorMessages.LastName.NOT_FOUND, operation.Id)
                        }
                    });
                    continue;
                }

                // Apply status update
                if (!string.IsNullOrWhiteSpace(operation.Patch.Status))
                {
                    if (!Enum.TryParse<LastNameStatus>(operation.Patch.Status, true, out var newStatus))
                    {
                        result.Failed++;
                        result.Results.Add(new LastNamesBatchItemResult
                        {
                            Id = operation.Id,
                            Status = StatusContants.ERROR,
                            Error = new LastNamesBatchError
                            {
                                Type = ProblemDetailsConstants.Types.RFC_BAD_REQUEST,
                                Title = ProblemDetailsConstants.Titles.BAD_REQUEST,
                                Status = 400,
                                Detail = StringFormatter.FormatMessage(ErrorMessages.LastName.INVALID_STATUS, operation.Patch.Status)
                            }
                        });
                        continue;
                    }

                    lastName.UpdateStatus(newStatus, operation.Patch.DiscontinueReason);
                }

                // Apply customer transfer
                if (operation.Patch.CustomerId.HasValue && operation.Patch.CustomerId.Value != lastName.CustomerId)
                {
                    lastName.TransferToCustomer(operation.Patch.CustomerId.Value);
                }

                await _repository.UpdateAsync(lastName, cancellationToken);

                result.Successful++;
                result.Results.Add(new LastNamesBatchItemResult
                {
                    Id = operation.Id,
                    Status = StatusContants.SUCCESS,
                    Resource = new
                    {
                        id = lastName.LastId,
                        status = lastName.Status.ToString(),
                        customerId = lastName.CustomerId,
                        updatedAt = lastName.UpdatedAt
                    }
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                result.Failed++;
                result.Results.Add(new LastNamesBatchItemResult
                {
                    Id = operation.Id,
                    Status = StatusContants.ERROR,
                    Error = new LastNamesBatchError
                    {
                        Type = ProblemDetailsConstants.Types.RFC_PRECONDITION_FAILED,
                        Title = ProblemDetailsConstants.Titles.PRECONDITION_FAILED,
                        Status = 412,
                        Detail = ErrorMessages.LastName.RESOURCE_WAS_MODIFIED_BY_ANOTHER_REQUEST
                    }
                });
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.Results.Add(new LastNamesBatchItemResult
                {
                    Id = operation.Id,
                    Status = StatusContants.ERROR,
                    Error = new LastNamesBatchError
                    {
                        Type = ProblemDetailsConstants.Types.RFC_INTERNAL_SERVER_ERROR,
                        Title = ProblemDetailsConstants.Titles.INTERNAL_SERVER_ERROR,
                        Status = 500,
                        Detail = ex.Message
                    }
                });
            }
        }

        return result;
    }
}