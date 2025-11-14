using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.Locations.DTOs;

public sealed record CreateLocationRequest
{
    [Required(ErrorMessage = "Location code is required")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "Location code must be between 1 and 20 characters")]
    public string LocationCode { get; init; } = string.Empty;

    [Required(ErrorMessage = "Location name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Location name must be between 1 and 100 characters")]
    public string LocationName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Location type is required")]
    [RegularExpression("^(Production|Development|Quality|Storage)$",
        ErrorMessage = "Location type must be Production, Development, Quality, or Storage")]
    public string LocationType { get; init; } = "Production";
}