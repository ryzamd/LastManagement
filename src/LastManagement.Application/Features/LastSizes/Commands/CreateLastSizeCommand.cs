using LastManagement.Application.Features.LastSizes.DTOs;
using LastManagement.Application.Features.LastSizes.Interfaces;
using LastManagement.Domain.LastSizes;

namespace LastManagement.Application.Features.LastSizes.Commands;

public class CreateLastSizeCommand
{
    private readonly ILastSizeRepository _repository;

    public CreateLastSizeCommand(ILastSizeRepository repository)
    {
        _repository = repository;
    }

    public async Task<LastSize> ExecuteAsync(CreateLastSizeRequest request, CancellationToken cancellationToken = default)
    {
        // Check for duplicate size value
        if (await _repository.ExistsAsync(request.SizeValue, cancellationToken))
        {
            throw new InvalidOperationException($"Size value {request.SizeValue} already exists");
        }

        var lastSize = LastSize.Create(request.SizeValue, request.SizeLabel);

        await _repository.AddAsync(lastSize, cancellationToken);

        return lastSize;
    }
}