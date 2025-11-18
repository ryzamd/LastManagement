using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.LastNames.DTOs
{
    public class BatchUpdateOperation
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ID must be positive")]
        public int Id { get; set; }

        [Required]
        public BatchUpdatePatch Patch { get; set; } = new();
    }
}
