using Asp.Versioning;
using LastManagement.Application.Features.Locations.Commands;
using LastManagement.Application.Features.Locations.DTOs;
using LastManagement.Application.Features.Locations.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LastManagement.Api.Features.Locations;

[ApiController]
[Route("api/v1/locations")]
[ApiVersion("1.0")]
public sealed class LocationsController : ControllerBase
{
    private readonly GetLocationsQueryHandler _getLocationsHandler;
    private readonly GetLocationByIdQueryHandler _getLocationByIdHandler;
    private readonly CreateLocationCommandHandler _createLocationHandler;
    private readonly UpdateLocationCommandHandler _updateLocationHandler;
    private readonly DeleteLocationCommandHandler _deleteLocationHandler;

    public LocationsController(
        GetLocationsQueryHandler getLocationsHandler,
        GetLocationByIdQueryHandler getLocationByIdHandler,
        CreateLocationCommandHandler createLocationHandler,
        UpdateLocationCommandHandler updateLocationHandler,
        DeleteLocationCommandHandler deleteLocationHandler)
    {
        _getLocationsHandler = getLocationsHandler;
        _getLocationByIdHandler = getLocationByIdHandler;
        _createLocationHandler = createLocationHandler;
        _updateLocationHandler = updateLocationHandler;
        _deleteLocationHandler = deleteLocationHandler;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<LocationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLocations(
        [FromQuery(Name = "$filter")] string? filter = null,
        CancellationToken cancellationToken = default)
    {
        string? filterType = null;
        bool? filterActive = null;

        if (!string.IsNullOrEmpty(filter))
        {
            var typeMatch = System.Text.RegularExpressions.Regex.Match(
                filter, @"locationType\s+eq\s+'(\w+)'", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (typeMatch.Success) filterType = typeMatch.Groups[1].Value;

            var activeMatch = System.Text.RegularExpressions.Regex.Match(
                filter, @"isActive\s+eq\s+(true|false)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (activeMatch.Success) filterActive = bool.Parse(activeMatch.Groups[1].Value);
        }

        var query = new GetLocationsQuery(filterType, filterActive);
        var result = await _getLocationsHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LocationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLocationById(int id, CancellationToken cancellationToken)
    {
        var query = new GetLocationByIdQuery(id);
        var result = await _getLocationByIdHandler.HandleAsync(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new ProblemDetails
            {
                Type = "http://localhost:5000/problems/not-found",
                Title = "Resource Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = result.Error,
                Instance = $"/api/v1/locations/{id}"
            });
        }

        Response.Headers.CacheControl = "public, max-age=60";
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(LocationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateLocation([FromBody] CreateLocationRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(request.LocationCode, request.LocationName, request.LocationType);
        var result = await _createLocationHandler.HandleAsync(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error!.Contains("already exists"))
            {
                return Conflict(new ProblemDetails
                {
                    Type = "http://localhost:5000/problems/duplicate-resource",
                    Title = "Duplicate Resource",
                    Status = StatusCodes.Status409Conflict,
                    Detail = result.Error,
                    Instance = "/api/v1/locations"
                });
            }
            return BadRequest(result.Error);
        }

        Response.Headers.Location = $"/api/v1/locations/{result.Value!.Id}";
        return CreatedAtAction(nameof(GetLocationById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(LocationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateLocationRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateLocationCommand(id, request.LocationName, request.LocationType, request.IsActive);
        var result = await _updateLocationHandler.HandleAsync(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Location not found")
            {
                return NotFound(new ProblemDetails
                {
                    Type = "http://localhost:5000/problems/not-found",
                    Title = "Resource Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = result.Error,
                    Instance = $"/api/v1/locations/{id}"
                });
            }
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteLocation(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteLocationCommand(id);
        var result = await _deleteLocationHandler.HandleAsync(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == "Location not found")
            {
                return NotFound(new ProblemDetails
                {
                    Type = "http://localhost:5000/problems/not-found",
                    Title = "Resource Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = result.Error,
                    Instance = $"/api/v1/locations/{id}"
                });
            }

            if (result.Error!.Contains("has inventory"))
            {
                return Conflict(new ProblemDetails
                {
                    Type = "http://localhost:5000/problems/conflict",
                    Title = "Conflict",
                    Status = StatusCodes.Status409Conflict,
                    Detail = result.Error,
                    Instance = $"/api/v1/locations/{id}"
                });
            }

            return BadRequest(result.Error);
        }

        return NoContent();
    }
}