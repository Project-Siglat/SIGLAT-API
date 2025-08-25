using System;
using System.ComponentModel.DataAnnotations;

namespace Craftmatrix.org.Model
{
    public class ReportDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [StringLength(200)]
        public string IncidentType { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
        
        [Required]
        public Guid WhoReportedId { get; set; }
        
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        [StringLength(1000)]
        public string? InvolvedAgencies { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property for reported user info
        public string? ReporterName { get; set; }
        public string? ReporterEmail { get; set; }
    }
}