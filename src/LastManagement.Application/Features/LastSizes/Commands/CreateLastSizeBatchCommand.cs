using LastManagement.Application.Features.LastSizes.DTOs;
using LastManagement.Application.Features.LastSizes.Interfaces;
using LastManagement.Domain.LastSizes;

namespace LastManagement.Application.Features.LastSizes.Commands;

public class CreateLastSizeBatchCommand
{
    private readonly ILastSizeRepository _repository;

    public CreateLastSizeBatchCommand(ILastSizeRepository repository)
    {
        _repository = repository;
    }

    public async Task<BatchOperationResult> ExecuteAsync(CreateLastSizeBatchRequest request, CancellationToken cancellationToken = default)
    {
        var result = new BatchOperationResult();

        foreach (var sizeRequest in request.Sizes)
        {
            try
            {
                // Check duplicate
                if (await _repository.ExistsAsync(sizeRequest.SizeValue, cancellationToken))
                {
                    result.Failed++;
                    result.Results.Add(new BatchItemResult
                    {
                        Status = "error",
                        Data = new { sizeValue = sizeRequest.SizeValue },
                        Error = new BatchError
                        {
                            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10",
                            Title = "Conflict",
                            Status = 409,
                            Detail = $"Size value {sizeRequest.SizeValue} already exists"
                        }
                    });
                    continue;
                }

                var lastSize = LastSize.Create(sizeRequest.SizeValue, sizeRequest.SizeLabel);
                await _repository.AddAsync(lastSize, cancellationToken);

                result.Successful++;
                result.Results.Add(new BatchItemResult
                {
                    Id = lastSize.SizeId,
                    Status = "success",
                    Data = new
                    {
                        id = lastSize.SizeId,
                        sizeValue = lastSize.SizeValue,
                        sizeLabel = lastSize.SizeLabel
                    }
                });
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.Results.Add(new BatchItemResult
                {
                    Status = "error",
                    Data = new { sizeValue = sizeRequest.SizeValue },
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