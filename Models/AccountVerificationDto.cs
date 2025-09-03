using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Craftmatrix.org.API.Models
{
    public class AccountVerificationDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int VerificationTypeId { get; set; }

        [MaxLength(200)]
        public string DocumentNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string DocumentName { get; set; } = string.Empty;

        public byte[]? DocumentImage { get; set; }

        [MaxLength(50)]
        public string? ImageMimeType { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = VerificationStatus.Pending;

        [MaxLength(500)]
        public string? AdminNotes { get; set; }

        public Guid? ReviewedByUserId { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties (not mapped to database)
        [NotMapped]
        public VerificationTypeDto? VerificationType { get; set; }

        [NotMapped] 
        public IdentityDto? User { get; set; }

        [NotMapped]
        public IdentityDto? ReviewedByUser { get; set; }
    }
}