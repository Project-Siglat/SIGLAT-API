using System.ComponentModel.DataAnnotations;

namespace Craftmatrix.org.API.Models
{
    public class IdentityDto
    {
        public Guid Id { get; set; }
        
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
        
        public bool IsPhoneVerified { get; set; } = false;
        
        public DateTime? PhoneVerifiedAt { get; set; }
        
        [Required]
        public int RoleId { get; set; }
        
        // Navigation property
        public virtual RoleDto Role { get; set; }
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public bool IsEmailVerified { get; set; } = false;
        
        public DateTime? EmailVerifiedAt { get; set; }
        
        [Required]
        [MinLength(6)]
        public string HashPass { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
