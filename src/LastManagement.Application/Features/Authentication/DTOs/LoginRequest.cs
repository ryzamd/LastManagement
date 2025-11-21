using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.Authentication.DTOs;

public sealed record LoginRequest
{
    [Required(ErrorMessage = ValidationMessages.Authentication.USERNAME_REQUIRED)]
    [StringLength(50, MinimumLength = 3, ErrorMessage = ValidationMessages.Authentication.USERNAME_LENGTH)]
    public string Username { get; init; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.Authentication.PASSWORD_REQUIRED)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = ValidationMessages.Authentication.PASSWORD_MIN_LENGTH)]
    public string Password { get; init; } = string.Empty;
}