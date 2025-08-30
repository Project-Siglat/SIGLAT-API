using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Craftmatrix.org.API.Models
{
    [Table("Alerts")]
    public class AlertDto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        
        [ForeignKey("Responder")]
        public Guid? ResponderId { get; set; }
        
        [Required]
        [StringLength(500)]
        public string What { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";
        
        public DateTime? RespondedAt { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Longitude { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Latitude { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual IdentityDto User { get; set; }
        public virtual IdentityDto Responder { get; set; }
    }
}
