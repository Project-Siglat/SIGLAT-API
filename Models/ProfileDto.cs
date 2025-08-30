using System.ComponentModel.DataAnnotations;

namespace Rai.SIGLAT.API.Models
{
    public class ProfileDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]  
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public string? PhoneNumber { get; set; }
        
        public string? Department { get; set; }
        
        public string? Location { get; set; }
    }
}