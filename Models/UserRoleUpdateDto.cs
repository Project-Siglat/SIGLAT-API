using System.ComponentModel.DataAnnotations;

namespace Rai.SIGLAT.API.Models
{
    public class UserRoleUpdateDto
    {
        [Required]
        [StringLength(50)]
        public string Role { get; set; }
    }
}