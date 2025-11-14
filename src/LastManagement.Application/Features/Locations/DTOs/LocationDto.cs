namespace LastManagement.Application.Features.Locations.DTOs;

public sealed record LocationDto
{
    public int Id { get; init; }
    public string LocationCode { get; init; } = string.Empty;
    public string LocationName { get; init; } = string.Empty;
    public string LocationType { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}