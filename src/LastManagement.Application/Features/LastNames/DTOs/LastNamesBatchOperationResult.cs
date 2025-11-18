namespace LastManagement.Application.Features.LastNames.DTOs;

public class LastNamesBatchOperationResult
{
    public int Successful { get; set; }
    public int Failed { get; set; }
    public List<LastNamesBatchItemResult> Results { get; set; } = new();
}