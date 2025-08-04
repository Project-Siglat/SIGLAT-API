using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SIGLAT.API.Model;
using Craftmatrix.org.Model;

namespace SIGLAT.API.Controllers.Ambulance
{
    [ApiController]
    [ApiVersion("1.0")]
    // [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IPostgreService _db;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatController(IPostgreService db, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("contactable-ambulance")]
        public async Task<IActionResult> GetContactableAmbulances()
        {
            var ambulances = await _db.GetDataAsync<IdentityDto>("Identity");
            var contactableAmbulances = ambulances.Where(a => a.Role == "Ambulance").ToList();
            return Ok(contactableAmbulances);
        }

        [HttpGet("contactable-user")]
        public async Task<IActionResult> GetContactableUsers()
        {
            var users = await _db.GetDataAsync<IdentityDto>("Identity");
            var contactableUsers = users.Where(u => u.Role == "User").ToList();
            return Ok(contactableUsers);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendChatMessage([FromBody] ChatDto chat)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            var tokenData = jsonToken.Payload.Jti;

            chat.Id = Guid.NewGuid();
            chat.Sender = Guid.Parse(tokenData);
            chat.SentAt = DateTime.UtcNow;
            await _db.PostDataAsync<ChatDto>("Chat", chat, chat.Id);
            return Ok();
        }

        [HttpGet("all-messages")]
        public async Task<IActionResult> GetAllMessages()
        {
            var messages = await _db.GetDataAsync<ChatDto>("Chat");
            return Ok(messages);
        }
    }
}
