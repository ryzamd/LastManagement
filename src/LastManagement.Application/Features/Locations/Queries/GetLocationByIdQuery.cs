using LastManagement.Application.Common.Models;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.Locations.DTOs;
using LastManagement.Application.Features.Locations.Interfaces;

namespace LastManagement.Application.Features.Locations.Queries;

public sealed record GetLocationByIdQuery(int Id);

public sealed class GetLocationByIdQueryHandler
{
    private readonly ILocationRepository _locationRepository;

    public GetLocationByIdQueryHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<Result<LocationDto>> HandleAsync(GetLocationByIdQuery query, CancellationToken cancellationToken = default)
    {
        var location = await _locationRepository.GetByIdAsync(query.Id, cancellationToken);
        if (location == null)
        {
            return Result.Failure<LocationDto>(ErrorMessages.Location.NOT_FOUND);
        }

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