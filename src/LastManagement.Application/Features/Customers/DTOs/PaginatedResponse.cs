namespace LastManagement.Application.Features.Customers.DTOs;

public sealed record PaginatedResponse<T>
{
    public List<T> Value { get; init; } = new();
    public string? NextLink { get; init; }
    public int Count { get; init; }
}