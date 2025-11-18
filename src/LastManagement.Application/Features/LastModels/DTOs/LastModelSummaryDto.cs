namespace LastManagement.Application.Features.LastModels.DTOs;

public class LastModelSummaryDto
{
    public int Id { get; set; }
    public string ModelCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}