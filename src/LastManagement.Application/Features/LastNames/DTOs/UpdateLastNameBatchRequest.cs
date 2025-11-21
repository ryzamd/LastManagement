using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.LastNames.DTOs;

public class UpdateLastNameBatchRequest
{
    [Required]
    [MinLength(1, ErrorMessage = ValidationMessages.LastName.AT_LEAST_ONE_OPERATION)]
    public List<BatchUpdateOperation> Operations { get; set; } = new();
}