namespace LastManagement.Application.Features.LastSizes.DTOs;

public class LastSizeDto
{
    public int Id { get; set; }
    public decimal SizeValue { get; set; }
    public string SizeLabel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? ReplacementSizeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Dictionary<string, object> Links { get; set; } = new();
}