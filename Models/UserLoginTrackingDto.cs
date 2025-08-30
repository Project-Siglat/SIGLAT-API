using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Craftmatrix.org.API.Models
{
    [Table("UserLoginTracking")]
    public class UserLoginTrackingDto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("Identity")]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(45)]
        public string IpAddress { get; set; }

        [StringLength(500)]
        public string UserAgent { get; set; }

        [Required]
        public DateTime LoginTimestamp { get; set; } = DateTime.UtcNow;

        public DateTime? LogoutTimestamp { get; set; }

        [Required]
        [StringLength(20)]
        public string LoginStatus { get; set; } = "Success";

        [StringLength(200)]
        public string FailureReason { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual IdentityDto Identity { get; set; }
    }
}