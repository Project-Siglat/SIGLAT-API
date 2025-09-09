namespace Craftmatrix.org.API.Models
{
    public class AccountVerificationResponseDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int VerificationTypeId { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AdminNotes { get; set; }
        public Guid? ReviewedByUserId { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Verification Type Info
        public string? VerificationTypeName { get; set; }
        public string? VerificationTypeDescription { get; set; }
    }
}