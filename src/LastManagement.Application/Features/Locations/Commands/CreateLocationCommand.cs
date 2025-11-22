using LastManagement.Application.Common.Interfaces;
using LastManagement.Application.Common.Models;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.Locations.DTOs;
using LastManagement.Application.Features.Locations.Interfaces;
using LastManagement.Domain.Locations;
using LastManagement.Utilities.Helpers;

namespace LastManagement.Application.Features.Locations.Commands;

public sealed record CreateLocationCommand(string LocationCode, string LocationName, string LocationType);

public sealed class CreateLocationCommandHandler
{
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLocationCommandHandler(ILocationRepository locationRepository, IUnitOfWork unitOfWork)
    {
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LocationDto>> HandleAsync(CreateLocationCommand command, CancellationToken cancellationToken = default)
    {
        if (await _locationRepository.ExistsByCodeAsync(command.LocationCode, null, cancellationToken))
        {
            return Result.Failure<LocationDto>(StringFormatter.FormatMessage(ErrorMessages.Location.ALREADY_EXISTS, command.LocationCode));
        }

        if (!Enum.TryParse<LocationType>(command.LocationType, out var locationType))
        {
            return Result.Failure<LocationDto>(ErrorMessages.Location.INVALID_TYPE);
        }

        var location = Location.Create(command.LocationCode, command.LocationName, locationType);

        await _locationRepository.AddAsync(location, cancellationToken);
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