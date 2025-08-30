using System.ComponentModel.DataAnnotations;

namespace Craftmatrix.org.API.Models
{
    public class VerificationStatusUpdateDto
    {
        [Required]
        public string Status { get; set; }
        
        public string? Remarks { get; set; }
    }
}