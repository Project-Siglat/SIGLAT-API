using System.ComponentModel.DataAnnotations;

namespace Craftmatrix.org.Model
{
    public class RegisterDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [StringLength(50)]
        public string MiddleName { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        
        [Required]
        [StringLength(255)]
        public string Address { get; set; }
        
        [Required]
        public string Gender { get; set; }
        
        [Phone]
        public string PhoneNumber { get; set; }
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [MinLength(6)]
        public string HashPass { get; set; }
    }
}