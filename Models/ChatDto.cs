using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Craftmatrix.org.API.Models
{
    [Table("Chat")]
    public class ChatDto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [ForeignKey("SenderUser")]
        public Guid SenderId { get; set; }
        
        [Required]
        [ForeignKey("ReceiverUser")]
        public Guid ReceiverId { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Message { get; set; }
        
        [Required]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        

    }
}
