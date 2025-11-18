namespace LastManagement.Application.Features.LastNames.DTOs;

public class LastNameDto
{
    public int Id { get; set; }
    public string LastCode { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? DiscontinueReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Expansion fields
    public CustomerSummaryDto? Customer { get; set; }
    public List<ModelSummaryDto>? Models { get; set; }
    public InventorySummaryDto? InventorySummary { get; set; }
}