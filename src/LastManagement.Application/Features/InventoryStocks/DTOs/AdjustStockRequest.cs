using LastManagement.Domain.InventoryStocks;
using System.ComponentModel.DataAnnotations;
using LastManagement.Application.Constants;

namespace LastManagement.Application.Features.InventoryStocks.DTOs;
public class AdjustStockRequest
{
    [Required]
    public AdjustmentType AdjustmentType { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = ValidationMessages.Inventory.QUANTITY_MIN)]
    public int Quantity { get; set; }

    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;

    [StringLength(50)]
    public string? ReferenceNumber { get; set; }
}