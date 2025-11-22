using LastManagement.Application.Constants;
using LastManagement.Application.Features.LastSizes.Interfaces;
using LastManagement.Utilities.Helpers;

namespace LastManagement.Application.Features.LastSizes.Commands;

public class DeleteLastSizeCommand
{
    private readonly ILastSizeRepository _repository;

    public DeleteLastSizeCommand(ILastSizeRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(int sizeId, CancellationToken cancellationToken = default)
    {
        var lastSize = await _repository.GetByIdAsync(sizeId, cancellationToken)
            ?? throw new KeyNotFoundException(StringFormatter.FormatMessage(ErrorMessages.LastSize.NOT_FOUND, sizeId));

        // Check if size is used in inventory
        if (await _repository.HasInventoryAsync(sizeId, cancellationToken))
        {
            throw new InvalidOperationException(ErrorMessages.LastSize.DELETE_lAST_SIZE_IN_USE_ERROR);
        }

        await _repository.DeleteAsync(lastSize, cancellationToken);
    }
}