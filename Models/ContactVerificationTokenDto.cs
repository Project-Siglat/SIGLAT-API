using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Craftmatrix.org.API.Models
{
    [Table("ContactVerificationTokens")]
    public class ContactVerificationTokenDto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [ForeignKey("Identity")]
        public Guid UserId { get; set; }
        
        [Required]
        [StringLength(20)]
        public string VerificationType { get; set; } // "email" or "phone"
        
        [Required]
        [StringLength(500)]
        public string ContactValue { get; set; } // email address or phone number
        
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
    
    public static class ContactVerificationType
    {
        public const string Email = "email";
        public const string Phone = "phone";
        
        public static readonly string[] AllTypes = { Email, Phone };
        
        public static bool IsValidType(string type)
        {
            return AllTypes.Contains(type?.ToLower());
        }
    }
    
    public class SendVerificationCodeDto
    {
        [Required]
        [StringLength(20)]
        public string VerificationType { get; set; } // "email" or "phone"
        
        [Required]
        [StringLength(500)]
        public string ContactValue { get; set; } // email address or phone number
    }
    
    public class VerifyContactCodeDto
    {
        [Required]
        [StringLength(20)]
        public string VerificationType { get; set; } // "email" or "phone"
        
        [Required]
        [StringLength(500)]
        public string ContactValue { get; set; } // email address or phone number
        
        [Required]
        [StringLength(10)]
        public string VerificationCode { get; set; }
    }
}