using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.PurchaseOrders.DTOs;

public sealed class CreatePurchaseOrderRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Location ID must be positive")]
    public int LocationId { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Requested by must be between 1 and 200 characters")]
    public string RequestedBy { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Department must not exceed 200 characters")]
    public string? Department { get; set; }

    [StringLength(1000, ErrorMessage = "Notes must not exceed 1000 characters")]
    public string? Notes { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one item is required")]
    public List<CreatePurchaseOrderItemRequest> Items { get; set; } = new();
}