using System.ComponentModel.DataAnnotations;

namespace Craftmatrix.org.API.Models
{
    public class UserRoleUpdateDto
    {
        [Required]
        public int RoleId { get; set; }
    }
}