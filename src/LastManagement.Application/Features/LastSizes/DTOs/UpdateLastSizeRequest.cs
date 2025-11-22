using LastManagement.Application.Constants;
using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.LastSizes.DTOs;

public class UpdateLastSizeRequest
{
    [StringLength(20, MinimumLength = 1, ErrorMessage = ValidationMessages.LastSize.SIZE_LABEL_LENGTH)]
    public string? SizeLabel { get; set; }

    [RegularExpression(FormatConstants.RegexPatterns.LAST_SIZE_STATUS, ErrorMessage = ValidationMessages.LastSize.STATUS_INVALID)]
    public string? Status { get; set; }

    public int? ReplacementSizeId { get; set; }
}