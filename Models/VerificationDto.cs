namespace Craftmatrix.org.Model
{
    public class VerificationDto
    {
        public Guid Id { get; set; }
        public string B64Image { get; set; }
        public string VerificationType { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class VerificationDetailsDto
    {
        public Guid Id { get; set; }
        public string B64Image { get; set; }
        public string Name { get; set; }
        public string VerificationType { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
