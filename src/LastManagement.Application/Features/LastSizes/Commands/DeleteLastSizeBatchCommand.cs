using LastManagement.Application.Features.LastSizes.DTOs;
using LastManagement.Application.Features.LastSizes.Interfaces;

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
                        Status = "error",
                        Error = new BatchError
                        {
                            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                            Title = "Not Found",
                            Status = 404,
                            Detail = $"Last size with ID {sizeId} not found"
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
                        Status = "error",
                        Error = new BatchError
                        {
                            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10",
                            Title = "Conflict",
                            Status = 409,
                            Detail = "Size is in use in inventory",
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

                await _repository.DeleteAsync(lastSize, cancellationToken);

                result.Successful++;
                result.Results.Add(new BatchItemResult
                {
                    Id = sizeId,
                    Status = "success"
                });
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.Results.Add(new BatchItemResult
                {
                    Id = sizeId,
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