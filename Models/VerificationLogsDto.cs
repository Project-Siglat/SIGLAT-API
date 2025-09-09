using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Craftmatrix.org.API.Models
{
    [Table("VerificationLogs")]
    public class VerificationLogsDto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [ForeignKey("Verification")]
        public Guid VerificationId { get; set; }
        
        [Required]
        [ForeignKey("AdminIdentity")]
        public Guid AdminUserId { get; set; }
        
        [Required]
        [StringLength(20)]
        public string PreviousStatus { get; set; }
        
        [Required]
        [StringLength(20)]
        public string NewStatus { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Action { get; set; }
        
        [StringLength(1000)]
        public string? Reason { get; set; }
        
        [StringLength(500)]
        public string? AdminRemarks { get; set; }
        
        [Required]
        public DateTime ActionTimestamp { get; set; } = DateTime.UtcNow;
        
        [StringLength(45)]
        public string? IpAddress { get; set; }
        
        [StringLength(500)]
        public string? UserAgent { get; set; }
        

    }
    
    public static class VerificationLogAction
    {
        public const string StatusChanged = "status_changed";
        public const string Created = "created";
        public const string Updated = "updated";
        public const string Approved = "approved";
        public const string Rejected = "rejected";
        public const string UnderReview = "under_review";
        public const string Resubmitted = "resubmitted";
        public const string DocumentUpdated = "document_updated";
        public const string RemarksUpdated = "remarks_updated";
        
        public static readonly string[] AllActions = { 
            StatusChanged, Created, Updated, Approved, Rejected, 
            UnderReview, Resubmitted, DocumentUpdated, RemarksUpdated 
        };
        
        public static bool IsValidAction(string action)
        {
            return AllActions.Contains(action?.ToLower());
        }
    }
}