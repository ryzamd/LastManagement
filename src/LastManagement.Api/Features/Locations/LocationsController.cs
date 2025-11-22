using Asp.Versioning;
using LastManagement.Api.Constants;
using LastManagement.Api.Global.Helpers;
using LastManagement.Application.Constants;
using LastManagement.Application.Features.Locations.Commands;
using LastManagement.Application.Features.Locations.DTOs;
using LastManagement.Application.Features.Locations.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

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
            var typeMatch = Regex.Match(filter, RegexPattern.Location.LOCATION_TYPE_FILTER, RegexOptions.IgnoreCase);
            if (typeMatch.Success) filterType = typeMatch.Groups[1].Value;

            var activeMatch = Regex.Match(filter, RegexPattern.Location.IS_ACTIVE_FILTER, RegexOptions.IgnoreCase);
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
                Type = ProblemDetailsConstants.Types.NOT_FOUND,
                Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                Status = StatusCodes.Status404NotFound,
                Detail = result.Error,
                Instance = UrlHelper.FormatInstancePath(ApiRoutes.Locations.FULL_BY_ID_TEMPLATE, id)
            });
        }

        Response.Headers.CacheControl = CacheConstants.CacheControl.PUBLIC_MAX_AGE_60;
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
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
                    Type = ProblemDetailsConstants.Types.DUPLICATE_RESOURCE,
                    Title = ProblemDetailsConstants.Titles.DUPLICATE_RESOURCE,
                    Status = StatusCodes.Status409Conflict,
                    Detail = result.Error,
                    Instance = ApiRoutes.Locations.FULL_BASE
                });
            }
            return BadRequest(result.Error);
        }

        Response.Headers.Location = UrlHelper.FormatResourceUrl(ApiRoutes.Locations.FULL_BY_ID_TEMPLATE, result.Value!.Id);
        return CreatedAtAction(nameof(GetLocationById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    [ProducesResponseType(typeof(LocationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateLocationRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateLocationCommand(id, request.LocationName, request.LocationType, request.IsActive);
        var result = await _updateLocationHandler.HandleAsync(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == ErrorMessages.Location.NOT_FOUND)
            {
                return NotFound(new ProblemDetails
                {
                    Type = ProblemDetailsConstants.Types.NOT_FOUND,
                    Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                    Status = StatusCodes.Status404NotFound,
                    Detail = result.Error,
                    Instance = UrlHelper.FormatInstancePath(ApiRoutes.Locations.FULL_BY_ID_TEMPLATE, id)
                });
            }
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AuthorizationConstants.Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteLocation(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteLocationCommand(id);
        var result = await _deleteLocationHandler.HandleAsync(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error == ErrorMessages.Location.NOT_FOUND)
            {
                return NotFound(new ProblemDetails
                {
                    Type = ProblemDetailsConstants.Types.NOT_FOUND,
                    Title = ProblemDetailsConstants.Titles.NOT_FOUND,
                    Status = StatusCodes.Status404NotFound,
                    Detail = result.Error,
                    Instance = UrlHelper.FormatInstancePath(ApiRoutes.Locations.FULL_BY_ID_TEMPLATE, id)
                });
            }

            if (result.Error!.Contains("has inventory"))
            {
                return Conflict(new ProblemDetails
                {
                    Type = ProblemDetailsConstants.Types.CONFLICT,
                    Title = ProblemDetailsConstants.Titles.CONFLICT,
                    Status = StatusCodes.Status409Conflict,
                    Detail = result.Error,
                    Instance = UrlHelper.FormatInstancePath(ApiRoutes.Locations.FULL_BY_ID_TEMPLATE, id)
                });
            }

            return BadRequest(result.Error);
        }

        return NoContent();
    }
}