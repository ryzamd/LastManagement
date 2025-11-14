namespace LastManagement.Application.Features.Customers.DTOs;

public sealed record CustomerDto
{
    public int Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public int Version { get; init; }
}