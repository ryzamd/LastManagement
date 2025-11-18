using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.LastNames.DTOs;

public class UpdateLastNameRequest
{
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Last code must be between 1 and 50 characters")]
    public string? LastCode { get; set; }

    public string? Status { get; set; }

    [StringLength(500, ErrorMessage = "Discontinue reason cannot exceed 500 characters")]
    public string? DiscontinueReason { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be positive")]
    public int? CustomerId { get; set; }
}