namespace Craftmatrix.org.Model
{
    public class AlertDto
    {
        public Guid Id { get; set; }
        public Guid Uid { get; set; }
        public Guid Responder { get; set; }
        public string What { get; set; }
        public string Status { get; set; }
        public DateTime RespondedAt { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
}
