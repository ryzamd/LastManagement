namespace LastManagement.Application.Features.LastNames.DTOs
{
    public class LastNamesBatchError
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Detail { get; set; } = string.Empty;
    }
}
