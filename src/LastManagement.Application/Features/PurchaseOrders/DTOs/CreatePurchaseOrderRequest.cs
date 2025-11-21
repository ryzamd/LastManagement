using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.PurchaseOrders.DTOs;

public sealed class CreatePurchaseOrderRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = ValidationMessages.PurchaseOrder.LOCATION_ID_REQUIRED)]
    public int LocationId { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 1, ErrorMessage = ValidationMessages.PurchaseOrder.REQUESTED_BY_REQUIRED)]
    public string RequestedBy { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = ValidationMessages.PurchaseOrder.DEPARTMENT_MAX_LENGTH)]
    public string? Department { get; set; }

    [StringLength(1000, ErrorMessage = ValidationMessages.PurchaseOrder.NOTES_MAX_LENGTH)]
    public string? Notes { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = ValidationMessages.PurchaseOrder.AT_LEAST_ONE_ITEM)]
    public List<CreatePurchaseOrderItemRequest> Items { get; set; } = new();
}