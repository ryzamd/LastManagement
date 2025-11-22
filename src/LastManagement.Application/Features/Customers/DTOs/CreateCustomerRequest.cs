using LastManagement.Application.Constants;
using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.Customers.DTOs;

public sealed record CreateCustomerRequest
{
    [Required(ErrorMessage = ValidationMessages.Customer.CUSTOMER_NAME_REQUIRED)]
    [StringLength(200, MinimumLength = 1, ErrorMessage = ValidationMessages.Customer.CUSTOMER_NAME_LENGTH)]
    public string CustomerName { get; init; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.Customer.STATUS_REQUIRED)]
    [RegularExpression(FormatConstants.RegexPatterns.CUSTOMER_STATUS, ErrorMessage = ValidationMessages.Customer.STATUS_INVALID)]
    public string Status { get; init; } = StatusConstants.Customer.ACTIVE;
}