namespace LastManagement.Application.Features.LastSizes.DTOs;

public class LastSizesBatchOperationResult
{
    public int Successful { get; set; }
    public int Failed { get; set; }
    public List<BatchItemResult> Results { get; set; } = new();
}