namespace Craftmatrix.org.Model
{
    public class ChatDto
    {
        public Guid Id { get; set; }
        public Guid Sender { get; set; }
        public Guid Reciever { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
    }
}
