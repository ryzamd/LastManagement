using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.LastSizes.DTOs;

public class DeleteLastSizeBatchRequest
{
    [Required]
    [MinLength(1, ErrorMessage = ValidationMessages.LastSize.AT_LEAST_ONE_ID)]
    public List<int> Ids { get; set; } = new();
}