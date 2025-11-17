namespace LastManagement.Application.Features.LastSizes.DTOs;

public class BatchOperationResult
{
    public int Successful { get; set; }
    public int Failed { get; set; }
    public List<BatchItemResult> Results { get; set; } = new();
}

public class BatchItemResult
{
    public int? Id { get; set; }
    public string Status { get; set; } = string.Empty; // "success" or "error"
    public object? Data { get; set; }
    public BatchError? Error { get; set; }
}

public class BatchError
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string Detail { get; set; } = string.Empty;
    public Dictionary<string, object>? AdditionalData { get; set; }
}