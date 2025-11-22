using LastManagement.Application.Constants;
using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.Customers.DTOs;

public sealed record UpdateCustomerRequest
{
    [StringLength(200, MinimumLength = 1, ErrorMessage = ValidationMessages.Customer.CUSTOMER_NAME_LENGTH)]
    public string? CustomerName { get; init; }

    [RegularExpression(FormatConstants.RegexPatterns.CUSTOMER_STATUS, ErrorMessage = ValidationMessages.Customer.STATUS_INVALID)]
    public string? Status { get; init; }
}