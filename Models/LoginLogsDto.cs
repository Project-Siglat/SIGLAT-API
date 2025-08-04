namespace Craftmatrix.org.Model
{
    public class LoginLogsDto
    {
        public Guid Id { get; set; }
        public Guid Who { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
