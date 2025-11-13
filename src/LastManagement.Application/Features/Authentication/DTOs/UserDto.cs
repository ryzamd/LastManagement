namespace LastManagement.Application.Features.Authentication.DTOs;

public sealed record UserDto
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime? LastLoginAt { get; init; }
    public DateTime CreatedAt { get; init; }
}