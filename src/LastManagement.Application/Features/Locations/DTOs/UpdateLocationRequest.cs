using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.Locations.DTOs;

public sealed record UpdateLocationRequest
{
    [StringLength(100, MinimumLength = 1, ErrorMessage = ValidationMessages.Location.NAME_LENGTH)]
    public string? LocationName { get; init; }

    [RegularExpression("^(Production|Development|Quality|Storage)$",
        ErrorMessage = ValidationMessages.Location.TYPE_INVALID)]
    public string? LocationType { get; init; }

    public bool? IsActive { get; init; }
}