using LastManagement.Application.Constants;
using System.ComponentModel.DataAnnotations;

namespace LastManagement.Application.Features.LastNames.DTOs
{
    public class BatchUpdateOperation
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = ErrorMessages.LastName.ID_MUST_BE_POSITIVE)]
        public int Id { get; set; }

        [Required]
        public BatchUpdatePatch Patch { get; set; } = new();
    }
}
