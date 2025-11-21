using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.LastSizes.DTOs;

public class CreateLastSizeRequest
{
    [Required(ErrorMessage = ValidationMessages.LastSize.SIZE_VALUE_REQUIRED)]
    [Range(0.1, 99.9, ErrorMessage = ValidationMessages.LastSize.SIZE_VALUE_RANGE)]
    public decimal SizeValue { get; set; }

    [Required(ErrorMessage = ValidationMessages.LastSize.SIZE_LABEL_REQUIRED)]
    [StringLength(20, MinimumLength = 1, ErrorMessage = ValidationMessages.LastSize.SIZE_LABEL_LENGTH)]
    public string SizeLabel { get; set; } = string.Empty;
}