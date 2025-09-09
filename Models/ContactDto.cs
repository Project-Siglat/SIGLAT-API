using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Craftmatrix.org.API.Models
{
    [Table("Contact")]
    public class ContactDto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [ForeignKey("Identity")]
        public Guid? UserId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Type { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Value { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        

    }
}
