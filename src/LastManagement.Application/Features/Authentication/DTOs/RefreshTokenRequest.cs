using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.Authentication.DTOs;

public sealed record RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; init; } = string.Empty;
}