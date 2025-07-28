namespace Craftmatrix.org.Model
{
    public class ContactDto
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public string ContactType { get; set; }
        public string ContactInformation { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
