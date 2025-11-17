using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.LastSizes.DTOs;

public class UpdateLastSizeRequest
{
    [StringLength(20, MinimumLength = 1, ErrorMessage = "Size label must be between 1 and 20 characters")]
    public string? SizeLabel { get; set; }

    [RegularExpression("^(Active|Discontinued|Replaced)$", ErrorMessage = "Status must be Active, Discontinued, or Replaced")]
    public string? Status { get; set; }

    public int? ReplacementSizeId { get; set; }
}