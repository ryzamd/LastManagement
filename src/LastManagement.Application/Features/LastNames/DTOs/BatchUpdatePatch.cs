namespace LastManagement.Application.Features.LastNames.DTOs
{
    public class BatchUpdatePatch
    {
        public string? Status { get; set; }
        public string? DiscontinueReason { get; set; }
        public int? CustomerId { get; set; }
    }
}
