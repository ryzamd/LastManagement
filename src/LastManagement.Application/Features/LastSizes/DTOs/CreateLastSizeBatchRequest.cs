using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.LastSizes.DTOs;

public class CreateLastSizeBatchRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one size is required")]
    public List<CreateLastSizeRequest> Sizes { get; set; } = new();
}