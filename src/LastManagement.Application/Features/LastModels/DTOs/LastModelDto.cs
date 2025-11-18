namespace LastManagement.Application.Features.LastModels.DTOs;

public class LastModelDto
{
    public int Id { get; set; }
    public int LastId { get; set; }
    public string ModelCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public object? Links { get; set; }
}