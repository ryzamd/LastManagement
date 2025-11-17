using LastManagement.Application.Features.LastSizes.Interfaces;

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
            ?? throw new KeyNotFoundException($"Last size with ID {sizeId} not found");

        // Check if size is used in inventory
        if (await _repository.HasInventoryAsync(sizeId, cancellationToken))
        {
            throw new InvalidOperationException("Cannot delete size because it is used in inventory");
        }

        await _repository.DeleteAsync(lastSize, cancellationToken);
    }
}