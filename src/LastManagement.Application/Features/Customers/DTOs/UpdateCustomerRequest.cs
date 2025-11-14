using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.Customers.DTOs;

public sealed record UpdateCustomerRequest
{
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Customer name must be between 1 and 200 characters")]
    public string? CustomerName { get; init; }

    [RegularExpression("^(Active|Inactive|Suspended)$", ErrorMessage = "Status must be Active, Inactive, or Suspended")]
    public string? Status { get; init; }
}