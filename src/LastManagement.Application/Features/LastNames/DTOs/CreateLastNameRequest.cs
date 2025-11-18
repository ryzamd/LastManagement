using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.LastNames.DTOs;

public class CreateLastNameRequest
{
    [Required(ErrorMessage = "Customer ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be positive")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Last code is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Last code must be between 1 and 50 characters")]
    public string LastCode { get; set; } = string.Empty;
}