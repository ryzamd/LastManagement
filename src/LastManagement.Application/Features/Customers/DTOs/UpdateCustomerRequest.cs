using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.Customers.DTOs;

public sealed record UpdateCustomerRequest
{
    [StringLength(200, MinimumLength = 1, ErrorMessage = ValidationMessages.Customer.CUSTOMER_NAME_LENGTH)]
    public string? CustomerName { get; init; }

    [RegularExpression("^(Active|Inactive|Suspended)$", ErrorMessage = ValidationMessages.Customer.STATUS_INVALID)]
    public string? Status { get; init; }
}