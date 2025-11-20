using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.PurchaseOrders.DTOs;

public sealed class ConfirmOrderRequest
{
    [StringLength(1000, ErrorMessage = "Admin notes must not exceed 1000 characters")]
    public string? AdminNotes { get; set; }
}