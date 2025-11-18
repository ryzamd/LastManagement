using LastManagement.Application.Features.LastNames.DTOs;
using LastManagement.Application.Features.LastNames.Interfaces;
using LastManagement.Domain.LastNames.Enums;
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
                        Status = "error",
                        Error = new LastNamesBatchError
                        {
                            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                            Title = "Not Found",
                            Status = 404,
                            Detail = $"Last name with ID {operation.Id} not found"
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
                            Status = "error",
                            Error = new LastNamesBatchError
                            {
                                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                                Title = "Bad Request",
                                Status = 400,
                                Detail = $"Invalid status value: {operation.Patch.Status}"
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
                    Status = "success",
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
                    Status = "error",
                    Error = new LastNamesBatchError
                    {
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.13",
                        Title = "Precondition Failed",
                        Status = 412,
                        Detail = "The resource was modified by another request. Please refresh and retry."
                    }
                });
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.Results.Add(new LastNamesBatchItemResult
                {
                    Id = operation.Id,
                    Status = "error",
                    Error = new LastNamesBatchError
                    {
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                        Title = "Internal Server Error",
                        Status = 500,
                        Detail = ex.Message
                    }
                });
            }
        }

        return result;
    }
}