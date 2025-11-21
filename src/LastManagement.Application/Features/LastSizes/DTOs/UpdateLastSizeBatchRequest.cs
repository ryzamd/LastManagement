using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.LastSizes.DTOs;

public class UpdateLastSizeBatchRequest
{
    [Required]
    [MinLength(1, ErrorMessage = ValidationMessages.LastSize.AT_LEAST_ONE_OPERATION)]
    public List<UpdateLastSizeBatchOperation> Operations { get; set; } = new();
}

public class UpdateLastSizeBatchOperation
{
    [Required]
    public int Id { get; set; }

    [Required]
    public UpdateLastSizeRequest Patch { get; set; } = new();
}