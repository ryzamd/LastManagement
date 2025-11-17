using System.ComponentModel.DataAnnotations;

public class BatchAdjustmentRequest
{
    [StringLength(50)]
    public string? ReferenceNumber { get; set; }

    [Required]
    [MinLength(1)]
    public List<BatchAdjustmentOperation> Operations { get; set; } = new();
}