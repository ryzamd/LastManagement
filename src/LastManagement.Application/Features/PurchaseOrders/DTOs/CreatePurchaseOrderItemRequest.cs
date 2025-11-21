using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.PurchaseOrders.DTOs;

public sealed class CreatePurchaseOrderItemRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = ValidationMessages.PurchaseOrderItem.LAST_ID_REQUIRED)]
    public int LastId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = ValidationMessages.PurchaseOrderItem.SIZE_ID_REQUIRED)]
    public int SizeId { get; set; }

    [Required]
    [Range(1, 999999, ErrorMessage = ValidationMessages.PurchaseOrderItem.QUANTITY_RANGE)]
    public int QuantityRequested { get; set; }
}