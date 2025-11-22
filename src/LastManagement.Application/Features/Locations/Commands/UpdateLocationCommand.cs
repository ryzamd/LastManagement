using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Common.Models;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.Locations.DTOs;
using LastManagement.Application.Features.Locations.Interfaces;
using LastManagement.Domain.Locations;

namespace LastManagement.Application.Features.Locations.Commands;

public sealed record UpdateLocationCommand(int Id, string? LocationName, string? LocationType, bool? IsActive);

public sealed class UpdateLocationCommandHandler
{
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLocationCommandHandler(ILocationRepository locationRepository, IUnitOfWork unitOfWork)
    {
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LocationDto>> HandleAsync(UpdateLocationCommand command, CancellationToken cancellationToken = default)
    {
        var location = await _locationRepository.GetByIdForUpdateAsync(command.Id, cancellationToken);
        if (location == null)
        {
            return Result.Failure<LocationDto>(ErrorMessages.Location.NOT_FOUND);
        }

        if (!string.IsNullOrWhiteSpace(command.LocationName))
        {
            location.UpdateName(command.LocationName);
        }

        if (!string.IsNullOrWhiteSpace(command.LocationType))
        {
            if (!Enum.TryParse<LocationType>(command.LocationType, out var newType))
            {
                return Result.Failure<LocationDto>(ErrorMessages.Location.INVALID_TYPE);
            }
            location.UpdateType(newType);
        }

        if (command.IsActive.HasValue)
        {
            if (command.IsActive.Value)
                location.Activate();
            else
                location.Deactivate();
        }

        _locationRepository.Update(location);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new LocationDto
        {
            Id = location.Id,
            LocationCode = location.LocationCode,
            LocationName = location.LocationName,
            LocationType = location.LocationType.ToString(),
            IsActive = location.IsActive,
            CreatedAt = location.CreatedAt
        };

        return Result.Success(dto);
    }
}