using System.ComponentModel.DataAnnotations;

public class TransferStockRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int LastId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int SizeId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int FromLocationId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int ToLocationId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;

    [StringLength(50)]
    public string? ReferenceNumber { get; set; }
}