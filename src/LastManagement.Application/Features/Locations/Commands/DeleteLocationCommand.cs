using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Common.Models;
using LastManagement.Application.Features.Locations.Interfaces;

namespace LastManagement.Application.Features.Locations.Commands;

public sealed record DeleteLocationCommand(int Id);

public sealed class DeleteLocationCommandHandler
{
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLocationCommandHandler(ILocationRepository locationRepository, IUnitOfWork unitOfWork)
    {
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteLocationCommand command, CancellationToken cancellationToken = default)
    {
        var location = await _locationRepository.GetByIdForUpdateAsync(command.Id, cancellationToken);
        if (location == null)
        {
            return Result.Failure("Location not found");
        }

        if (await _locationRepository.HasInventoryAsync(command.Id, cancellationToken))
        {
            return Result.Failure("Cannot delete location because it has inventory");
        }

        _locationRepository.Delete(location);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}