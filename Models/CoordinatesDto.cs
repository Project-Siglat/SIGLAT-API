using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Craftmatrix.org.API.Models
{
    [Table("Coordinates")]
    public class CoordinatesDto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [ForeignKey("Identity")]
        public Guid UserId { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Latitude { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Longitude { get; set; }
        
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public virtual IdentityDto Identity { get; set; }
    }
}
