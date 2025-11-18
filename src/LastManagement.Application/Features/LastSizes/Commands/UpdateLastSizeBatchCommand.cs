using LastManagement.Application.Features.LastSizes.DTOs;
using LastManagement.Application.Features.LastSizes.Interfaces;
using LastManagement.Domain.LastSizes.Enums;

namespace LastManagement.Application.Features.LastSizes.Commands;

public class UpdateLastSizeBatchCommand
{
    private readonly ILastSizeRepository _repository;

    public UpdateLastSizeBatchCommand(ILastSizeRepository repository)
    {
        _repository = repository;
    }

    public async Task<LastSizesBatchOperationResult> ExecuteAsync(UpdateLastSizeBatchRequest request, CancellationToken cancellationToken = default)
    {
        var result = new LastSizesBatchOperationResult();

        foreach (var operation in request.Operations)
        {
            try
            {
                var lastSize = await _repository.GetByIdAsync(operation.Id, cancellationToken);

                if (lastSize == null)
                {
                    result.Failed++;
                    result.Results.Add(new BatchItemResult
                    {
                        Id = operation.Id,
                        Status = "error",
                        Error = new BatchError
                        {
                            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                            Title = "Not Found",
                            Status = 404,
                            Detail = $"Last size with ID {operation.Id} not found"
                        }
                    });
                    continue;
                }

                // Check if size has inventory - skip update if has inventory
                if (await _repository.HasInventoryAsync(operation.Id, cancellationToken))
                {
                    result.Failed++;
                    result.Results.Add(new BatchItemResult
                    {
                        Id = operation.Id,
                        Status = "error",
                        Error = new BatchError
                        {
                            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10",
                            Title = "Conflict",
                            Status = 409,
                            Detail = "Cannot update size that is used in inventory",
                            AdditionalData = new Dictionary<string, object>
                            {
                                ["conflictReason"] = "has-inventory",
                                ["sizeValue"] = lastSize.SizeValue,
                                ["sizeLabel"] = lastSize.SizeLabel
                            }
                        }
                    });
                    continue;
                }

                // Apply updates
                if (!string.IsNullOrWhiteSpace(operation.Patch.SizeLabel))
                {
                    lastSize.UpdateLabel(operation.Patch.SizeLabel);
                }

                if (!string.IsNullOrWhiteSpace(operation.Patch.Status))
                {
                    var newStatus = Enum.Parse<SizeStatus>(operation.Patch.Status);

                    if (newStatus == SizeStatus.Active && lastSize.Status != SizeStatus.Active)
                    {
                        lastSize.Reactivate();
                    }
                    else if (newStatus != SizeStatus.Active && lastSize.Status == SizeStatus.Active)
                    {
                        if (operation.Patch.ReplacementSizeId.HasValue)
                        {
                            var replacementSize = await _repository.GetByIdAsync(
                                operation.Patch.ReplacementSizeId.Value,
                                cancellationToken);

                            if (replacementSize == null)
                            {
                                result.Failed++;
                                result.Results.Add(new BatchItemResult
                                {
                                    Id = operation.Id,
                                    Status = "error",
                                    Error = new BatchError
                                    {
                                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                                        Title = "Bad Request",
                                        Status = 400,
                                        Detail = $"Replacement size with ID {operation.Patch.ReplacementSizeId.Value} not found"
                                    }
                                });
                                continue;
                            }
                        }

                        lastSize.Discontinue(operation.Patch.ReplacementSizeId);
                    }
                }

                await _repository.UpdateAsync(lastSize, cancellationToken);

                result.Successful++;
                result.Results.Add(new BatchItemResult
                {
                    Id = operation.Id,
                    Status = "success",
                    Data = new
                    {
                        id = lastSize.SizeId,
                        sizeValue = lastSize.SizeValue,
                        sizeLabel = lastSize.SizeLabel,
                        status = lastSize.Status.ToString()
                    }
                });
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.Results.Add(new BatchItemResult
                {
                    Id = operation.Id,
                    Status = "error",
                    Error = new BatchError
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