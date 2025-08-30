using System.ComponentModel.DataAnnotations;

namespace Rai.SIGLAT.API.Models
{
    public class VerificationStatusUpdateDto
    {
        [Required]
        public string Status { get; set; }
        
        public string? Remarks { get; set; }
    }
}