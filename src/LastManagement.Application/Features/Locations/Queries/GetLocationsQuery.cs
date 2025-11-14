using LastManagement.Application.Common.Models;
using LastManagement.Application.Features.Locations.DTOs;
using LastManagement.Application.Features.Locations.Interfaces;

namespace LastManagement.Application.Features.Locations.Queries;

public sealed record GetLocationsQuery(string? FilterType, bool? FilterActive);

public sealed class GetLocationsQueryHandler
{
    private readonly ILocationRepository _locationRepository;

    public GetLocationsQueryHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<Result<List<LocationDto>>> HandleAsync(GetLocationsQuery query, CancellationToken cancellationToken = default)
    {
        var locations = await _locationRepository.GetAllAsync(query.FilterType, query.FilterActive, cancellationToken);

        var dtos = locations.Select(l => new LocationDto
        {
            Id = l.Id,
            LocationCode = l.LocationCode,
            LocationName = l.LocationName,
            LocationType = l.LocationType.ToString(),
            IsActive = l.IsActive,
            CreatedAt = l.CreatedAt
        }).ToList();

        return Result.Success(dtos);
    }
}