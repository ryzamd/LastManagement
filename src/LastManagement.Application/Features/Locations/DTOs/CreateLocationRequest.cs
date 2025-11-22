using LastManagement.Application.Constants;
using System.ComponentModel.DataAnnotations;

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
    [RegularExpression(FormatConstants.RegexPatterns.LOCATION_TYPE, ErrorMessage = ValidationMessages.Location.TYPE_INVALID)]
    public string LocationType { get; init; } = StatusConstants.LocationType.PRODUCTION;
}