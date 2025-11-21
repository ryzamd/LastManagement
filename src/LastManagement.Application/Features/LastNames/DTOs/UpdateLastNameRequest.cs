using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.LastNames.DTOs;

public class UpdateLastNameRequest
{
    [StringLength(50, MinimumLength = 1, ErrorMessage = ValidationMessages.LastName.LAST_CODE_LENGTH)]
    public string? LastCode { get; set; }

    public string? Status { get; set; }

    [StringLength(500, ErrorMessage = ValidationMessages.LastName.DISCONTINUE_REASON_MAX_LENGTH)]
    public string? DiscontinueReason { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = ValidationMessages.LastName.CUSTOMER_ID_POSITIVE)]
    public int? CustomerId { get; set; }
}