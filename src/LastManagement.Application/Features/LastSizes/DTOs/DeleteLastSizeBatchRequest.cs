using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.LastSizes.DTOs;

public class DeleteLastSizeBatchRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one ID is required")]
    public List<int> Ids { get; set; } = new();
}