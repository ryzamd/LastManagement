namespace LastManagement.Application.Features.LastSizes.DTOs;

public class BatchOperationResult
{
    public int Successful { get; set; }
    public int Failed { get; set; }
    public List<BatchItemResult> Results { get; set; } = new();
}