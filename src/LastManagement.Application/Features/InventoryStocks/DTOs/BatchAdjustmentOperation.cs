using LastManagement.Domain.InventoryStocks;
using System.ComponentModel.DataAnnotations;

public class BatchAdjustmentOperation
{
    [Required]
    [Range(1, int.MaxValue)]
    public int StockId { get; set; }

    [Required]
    public AdjustmentType AdjustmentType { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;
}