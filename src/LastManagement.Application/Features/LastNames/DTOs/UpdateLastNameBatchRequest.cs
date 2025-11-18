using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.LastNames.DTOs;

public class UpdateLastNameBatchRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one operation is required")]
    public List<BatchUpdateOperation> Operations { get; set; } = new();
}