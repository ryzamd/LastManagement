using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.Authentication.DTOs;

public sealed record RefreshTokenRequest
{
    [Required(ErrorMessage = ValidationMessages.Authentication.REFRESH_TOKEN_REQUIRED)]
    public string RefreshToken { get; init; } = string.Empty;
}