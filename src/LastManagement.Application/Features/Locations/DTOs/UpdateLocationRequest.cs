using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.Locations.DTOs;

public sealed record UpdateLocationRequest
{
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Location name must be between 1 and 100 characters")]
    public string? LocationName { get; init; }

    [RegularExpression("^(Production|Development|Quality|Storage)$",
        ErrorMessage = "Location type must be Production, Development, Quality, or Storage")]
    public string? LocationType { get; init; }

    public bool? IsActive { get; init; }
}