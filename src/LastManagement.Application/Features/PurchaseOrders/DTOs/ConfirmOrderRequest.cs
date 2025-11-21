using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.PurchaseOrders.DTOs;

public sealed class ConfirmOrderRequest
{
    [StringLength(1000, ErrorMessage = ValidationMessages.PurchaseOrder.ADMIN_NOTES_MAX_LENGTH)]
    public string? AdminNotes { get; set; }
}