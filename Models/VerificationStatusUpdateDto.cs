using System.ComponentModel.DataAnnotations;

namespace Craftmatrix.org.Model
{
    public class VerificationStatusUpdateDto
    {
        [Required]
        public string Status { get; set; }
        
        public string? Remarks { get; set; }
    }
}