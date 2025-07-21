using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Craftmatrix.org.Model;
using Craftmatrix.Codex.org.Service;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SIGLATAPI.Controllers.WhoAmI
{
    [ApiController]
    [ApiVersion("1.0")]
    // [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IPostgreService _db;
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(IPostgreService db, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;

        }

        [HttpGet("userlist")]
        [AllowAnonymous]
        public async Task<IActionResult> UserLit()
        {
            var data = await _db.GetDataAsync<IdentityDto>("Identity");
            return Ok(data);
        }

        [HttpGet("contact")]
        public async Task<IActionResult> Contacts()
        {
            var data = await _db.GetDataAsync<ContactDto>("Contact");
            return Ok(data);
        }

        [HttpDelete("contact")]
        public async Task<IActionResult> DeleteContact(Guid Id)
        {
            await _db.DeleteDataAsync("Contact", Id);
            return Ok(Id);
        }

        [HttpPost("contact")]
        public async Task<IActionResult> Contact([FromBody] ContactDto Contact)
        {
            Contact.CreatedAt = DateTime.UtcNow;
            Contact.UpdatedAt = DateTime.UtcNow;
            await _db.PostDataAsync<ContactDto>("Contact", Contact, Contact.Id);
            return Ok(Contact);
        }
    }
}
