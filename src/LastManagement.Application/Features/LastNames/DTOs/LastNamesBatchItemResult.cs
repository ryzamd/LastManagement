namespace LastManagement.Application.Features.LastNames.DTOs
{
    public class LastNamesBatchItemResult
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public object? Resource { get; set; }
        public LastNamesBatchError? Error { get; set; }
    }
}
