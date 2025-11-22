using LastManagement.Api.Constants;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.LastSizes.DTOs;
using LastManagement.Application.Features.LastSizes.Interfaces;
using LastManagement.Domain.LastSizes;
using LastManagement.Utilities.Constants.Global;
using LastManagement.Utilities.Helpers;

namespace LastManagement.Application.Features.LastSizes.Commands;

public class CreateLastSizeBatchCommand
{
    private readonly ILastSizeRepository _repository;

    public CreateLastSizeBatchCommand(ILastSizeRepository repository)
    {
        _repository = repository;
    }

    public async Task<LastSizesBatchOperationResult> ExecuteAsync(CreateLastSizeBatchRequest request, CancellationToken cancellationToken = default)
    {
        var result = new LastSizesBatchOperationResult();

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
                        Status = StatusContants.ERROR,
                        Data = new { sizeValue = sizeRequest.SizeValue },
                        Error = new BatchError
                        {
                            Type = ProblemDetailsConstants.Types.RFC_CONFLICT,
                            Title = ProblemDetailsConstants.Titles.CONFLICT,
                            Status = 409,
                            Detail = StringFormatter.FormatMessage(ErrorMessages.LastSize.ALREADY_EXISTS, sizeRequest.SizeValue, sizeRequest.SizeLabel)
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
                    Status = StatusContants.SUCCESS,
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
                    Status = StatusContants.ERROR,
                    Data = new { sizeValue = sizeRequest.SizeValue },
                    Error = new BatchError
                    {
                        Type = ProblemDetailsConstants.Types.INTERNAL_SERVER_ERROR,
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