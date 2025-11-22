using LastManagement.Application.Constants;
using LastManagement.Application.Features.LastSizes.DTOs;
using LastManagement.Application.Features.LastSizes.Interfaces;
using LastManagement.Domain.LastSizes;
using LastManagement.Domain.LastSizes.Enums;
using LastManagement.Utilities.Helpers;

namespace LastManagement.Application.Features.LastSizes.Commands;

public class UpdateLastSizeCommand
{
    private readonly ILastSizeRepository _repository;

    public UpdateLastSizeCommand(ILastSizeRepository repository)
    {
        _repository = repository;
    }

    public async Task<LastSize> ExecuteAsync(int sizeId, UpdateLastSizeRequest request, CancellationToken cancellationToken = default)
    {
        var lastSize = await _repository.GetByIdAsync(sizeId, cancellationToken)
            ?? throw new KeyNotFoundException(StringFormatter.FormatMessage(ErrorMessages.LastSize.NOT_FOUND, sizeId));

        // Update label if provided
        if (!string.IsNullOrWhiteSpace(request.SizeLabel))
        {
            lastSize.UpdateLabel(request.SizeLabel);
        }

        // Update status if provided
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var newStatus = Enum.Parse<SizeStatus>(request.Status);

            if (newStatus == SizeStatus.Active && lastSize.Status != SizeStatus.Active)
            {
                lastSize.Reactivate();
            }
            else if (newStatus != SizeStatus.Active && lastSize.Status == SizeStatus.Active)
            {
                // Validate replacement size if provided
                if (request.ReplacementSizeId.HasValue)
                {
                    var replacementSize = await _repository.GetByIdAsync(request.ReplacementSizeId.Value, cancellationToken);
                    if (replacementSize == null)
                    {
                        throw new InvalidOperationException(StringFormatter.FormatMessage(ErrorMessages.LastSize.NOT_FOUND, request.ReplacementSizeId.Value));
                    }
                }

                lastSize.Discontinue(request.ReplacementSizeId);
            }
        }

        await _repository.UpdateAsync(lastSize, cancellationToken);

        return lastSize;
    }
}