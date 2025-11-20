using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.PurchaseOrders.DTOs;

public sealed class CreatePurchaseOrderItemRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Last ID must be positive")]
    public int LastId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Size ID must be positive")]
    public int SizeId { get; set; }

    [Required]
    [Range(1, 999999, ErrorMessage = "Quantity must be between 1 and 999999")]
    public int QuantityRequested { get; set; }
}