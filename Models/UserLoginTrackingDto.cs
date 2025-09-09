namespace Craftmatrix.org.API.Models
{
    public class UserLoginTrackingDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime LoginTimestamp { get; set; } = DateTime.UtcNow;
        public DateTime? LogoutTimestamp { get; set; }
        public string LoginStatus { get; set; } = "Success";
        public string? FailureReason { get; set; }
        public string? AttemptedEmail { get; set; }
        public bool IsActive { get; set; } = true;
    }
}