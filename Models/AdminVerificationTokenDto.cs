using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Craftmatrix.org.API.Models
{
    [Table("AdminVerificationTokens")]
    public class AdminVerificationTokenDto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [StringLength(500)]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [StringLength(10)]
        public string VerificationCode { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime ExpiresAt { get; set; }
        
        public bool IsUsed { get; set; } = false;
        
        public DateTime? VerifiedAt { get; set; }
        
        [StringLength(45)]
        public string? IpAddress { get; set; }
        
        [StringLength(500)]
        public string? UserAgent { get; set; }
        
        public int AttemptCount { get; set; } = 0;
    }
    
    public class SendAdminOtpDto
    {
        [Required]
        [EmailAddress]
        [StringLength(500)]
        public string Email { get; set; }
    }
    
    public class VerifyAdminOtpDto
    {
        [Required]
        [EmailAddress]
        [StringLength(500)]
        public string Email { get; set; }
        
        [Required]
        [StringLength(10)]
        public string VerificationCode { get; set; }
    }
    
    public class CreateAdminWithOtpDto
    {
        [Required]
        [EmailAddress]
        [StringLength(500)]
        public string Email { get; set; }
        
        [Required]
        [StringLength(10)]
        public string VerificationCode { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Password { get; set; }
    }
}