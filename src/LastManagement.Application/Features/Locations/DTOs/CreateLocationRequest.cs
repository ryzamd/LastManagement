using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.Locations.DTOs;

public sealed record CreateLocationRequest
{
    [Required(ErrorMessage = ValidationMessages.Location.CODE_REQUIRED)]
    [StringLength(20, MinimumLength = 1, ErrorMessage = ValidationMessages.Location.CODE_LENGTH)]
    public string LocationCode { get; init; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.Location.NAME_REQUIRED)]
    [StringLength(100, MinimumLength = 1, ErrorMessage = ValidationMessages.Location.NAME_LENGTH)]
    public string LocationName { get; init; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.Location.TYPE_REQUIRED)]
    [RegularExpression("^(Production|Development|Quality|Storage)$",
        ErrorMessage = ValidationMessages.Location.TYPE_INVALID)]
    public string LocationType { get; init; } = "Production";
}