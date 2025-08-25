using System.ComponentModel.DataAnnotations;

namespace Craftmatrix.org.Model
{
    public class UserRoleUpdateDto
    {
        [Required]
        [StringLength(50)]
        public string Role { get; set; }
    }
}