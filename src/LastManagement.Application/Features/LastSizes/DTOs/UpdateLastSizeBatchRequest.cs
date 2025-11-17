using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.LastSizes.DTOs;

public class UpdateLastSizeBatchRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one operation is required")]
    public List<UpdateLastSizeBatchOperation> Operations { get; set; } = new();
}

public class UpdateLastSizeBatchOperation
{
    [Required]
    public int Id { get; set; }

    [Required]
    public UpdateLastSizeRequest Patch { get; set; } = new();
}