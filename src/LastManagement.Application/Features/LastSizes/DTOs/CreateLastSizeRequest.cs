using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.LastSizes.DTOs;

public class CreateLastSizeRequest
{
    [Required(ErrorMessage = "Size value is required")]
    [Range(0.1, 99.9, ErrorMessage = "Size value must be between 0.1 and 99.9")]
    public decimal SizeValue { get; set; }

    [Required(ErrorMessage = "Size label is required")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "Size label must be between 1 and 20 characters")]
    public string SizeLabel { get; set; } = string.Empty;
}