using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.LastNames.DTOs;

public class CreateLastNameRequest
{
    [Required(ErrorMessage = ValidationMessages.LastName.CUSTOMER_ID_REQUIRED)]
    [Range(1, int.MaxValue, ErrorMessage = ValidationMessages.LastName.CUSTOMER_ID_POSITIVE)]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = ValidationMessages.LastName.LAST_CODE_REQUIRED)]
    [StringLength(50, MinimumLength = 1, ErrorMessage = ValidationMessages.LastName.LAST_CODE_LENGTH)]
    public string LastCode { get; set; } = string.Empty;
}