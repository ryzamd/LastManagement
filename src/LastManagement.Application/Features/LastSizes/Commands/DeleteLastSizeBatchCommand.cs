using LastManagement.Api.Constants;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.LastSizes.DTOs;
using LastManagement.Application.Features.LastSizes.Interfaces;
using LastManagement.Utilities.Constants.Global;
using LastManagement.Utilities.Helpers;

namespace LastManagement.Application.Features.LastSizes.Commands;

public class DeleteLastSizeBatchCommand
{
    private readonly ILastSizeRepository _repository;

    public DeleteLastSizeBatchCommand(ILastSizeRepository repository)
    {
        _repository = repository;
    }

    public async Task<LastSizesBatchOperationResult> ExecuteAsync(DeleteLastSizeBatchRequest request, CancellationToken cancellationToken = default)
    {
        var result = new LastSizesBatchOperationResult();

        foreach (var sizeId in request.Ids)
        {
            try
            {
                var lastSize = await _repository.GetByIdAsync(sizeId, cancellationToken);

                if (lastSize == null)
                {
                    result.Failed++;
                    result.Results.Add(new BatchItemResult
                    {
                        Id = sizeId,
                        Status = StatusContants.ERROR,
                        Error = new BatchError
                        {
                            Type = ProblemDetailsConstants.Types.RFC_NOT_FOUND,
                            Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                            Status = 404,
                            Detail = StringFormatter.FormatMessage(ErrorMessages.LastSize.NOT_FOUND, sizeId)
                        }
                    });
                    continue;
                }

                // Check if size is used in inventory
                if (await _repository.HasInventoryAsync(sizeId, cancellationToken))
                {
                    result.Failed++;
                    result.Results.Add(new BatchItemResult
                    {
                        Id = sizeId,
                        Status = StatusContants.ERROR,
                        Error = new BatchError
                        {
                            Type = ProblemDetailsConstants.Types.RFC_CONFLICT,
                            Title = ProblemDetailsConstants.Titles.CONFLICT,
                            Status = 409,
                            Detail = ErrorMessages.LastSize.SIZE_IS_IN_USE,
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

                await _repository.DeleteAsync(lastSize, cancellationToken);

                result.Successful++;
                result.Results.Add(new BatchItemResult
                {
                    Id = sizeId,
                    Status = StatusContants.SUCCESS,
                });
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.Results.Add(new BatchItemResult
                {
                    Id = sizeId,
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