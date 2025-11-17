public class BatchItemResult
{
    public int? Id { get; set; }
    public string Status { get; set; } = string.Empty; // "success" or "error"
    public object? Data { get; set; }
    public BatchError? Error { get; set; }
}