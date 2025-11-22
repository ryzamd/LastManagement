using LastManagement.Api.Constants;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.LastSizes.DTOs;
using LastManagement.Application.Features.LastSizes.Interfaces;
using LastManagement.Domain.LastSizes.Enums;
using LastManagement.Utilities.Constants.Global;
using LastManagement.Utilities.Helpers;

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
                        Status = StatusContants.ERROR,
                        Error = new BatchError
                        {
                            Type = ProblemDetailsConstants.Types.RFC_INTERNAL_SERVER_ERROR,
                            Title = ProblemDetailsConstants.Titles.INTERNAL_SERVER_ERROR,
                            Status = 404,
                            Detail = StringFormatter.FormatMessage(ErrorMessages.LastSize.NOT_FOUND, operation.Id)
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
                        Status = StatusContants.ERROR,
                        Error = new BatchError
                        {
                            Type = ProblemDetailsConstants.Types.RFC_CONFLICT,
                            Title = ProblemDetailsConstants.Titles.CONFLICT,
                            Status = 409,
                            Detail = ErrorMessages.LastSize.SIZE_IS_IN_USE_CANNOT_UPDATE,
                            AdditionalData = new Dictionary<string, object>
                            {
                                ["conflictReason"] = ConflictMessages.Reasons.HAS_INVENTORY,
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
                                    Status = StatusContants.ERROR,
                                    Error = new BatchError
                                    {
                                        Type = ProblemDetailsConstants.Types.RFC_BAD_REQUEST,
                                        Title = ProblemDetailsConstants.Titles.BAD_REQUEST,
                                        Status = 400,
                                        Detail = StringFormatter.FormatMessage(ErrorMessages.LastSize.NOT_FOUND, operation.Patch.ReplacementSizeId.Value)
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
                    Status = StatusContants.SUCCESS,
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
                    Status = StatusContants.ERROR,
                    Error = new BatchError
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