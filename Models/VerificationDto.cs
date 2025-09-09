using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Craftmatrix.org.API.Models
{
    [Table("Verifications")]
    public class VerificationDto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [ForeignKey("Identity")]
        public Guid UserId { get; set; }
        
        [Required]
        public string B64Image { get; set; }
        
        [Required]
        [StringLength(50)]
        public string VerificationType { get; set; }
        
        [StringLength(500)]
        public string Remarks { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        

    }

    public class VerificationDetailsDto
    {
        public Guid Id { get; set; }
        public string B64Image { get; set; }
        public string Name { get; set; }
        public string VerificationType { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // Email OTP DTOs for frontend compatibility
    public class SendEmailOtpDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class VerifyEmailDto
    {
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string VerificationCode { get; set; }
    }
}
