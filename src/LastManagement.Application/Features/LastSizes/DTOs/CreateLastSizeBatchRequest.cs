using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.LastSizes.DTOs;

public class CreateLastSizeBatchRequest
{
    [Required]
    [MinLength(1, ErrorMessage = ValidationMessages.LastSize.AT_LEAST_ONE_SIZE)]
    public List<CreateLastSizeRequest> Sizes { get; set; } = new();
}