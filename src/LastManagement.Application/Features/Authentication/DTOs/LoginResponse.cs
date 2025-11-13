namespace LastManagement.Application.Features.Authentication.DTOs;

public sealed record LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = "Bearer";
    public int ExpiresIn { get; init; }
    public string RefreshToken { get; init; } = string.Empty;
    public UserDto User { get; init; } = null!;
}