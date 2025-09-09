using System.ComponentModel.DataAnnotations;

namespace Craftmatrix.org.API.Models
{
    public class TypeOfIncidentDto
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string NameOfIncident { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public DateTime AddedDateTime { get; set; }
        
        [Required]
        public Guid WhoAddedItID { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public bool isBFPTrue { get; set; } = false;
        
        public bool isPNPTrue { get; set; } = false;
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
}