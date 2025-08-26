using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SIGLAT.API.Model;
using Craftmatrix.org.Model;

namespace SIGLAT.API.Controllers.Ambulance
{
    /// <summary>
    /// Controller for managing chat functionality and communication between users and emergency responders
    /// </summary>
    /// <remarks>
    /// This controller facilitates real-time communication between users, ambulances, and other emergency responders.
    /// It provides endpoints for retrieving contactable entities, sending messages, and accessing message history.
    /// </remarks>
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

        /// <summary>
        /// Retrieves a list of all ambulance personnel available for communication
        /// </summary>
        /// <returns>A list of ambulance personnel with their identity information</returns>
        /// <response code="200">Ambulance list retrieved successfully</response>
        /// <response code="500">Internal server error occurred while retrieving ambulance personnel</response>
        [HttpGet("contactable-ambulance")]
        public async Task<IActionResult> GetContactableAmbulances()
        {
            var ambulances = await _db.GetDataAsync<IdentityDto>("Identity");
            var contactableAmbulances = ambulances.Where(a => a.Role == "Ambulance").ToList();
            return Ok(contactableAmbulances);
        }

        /// <summary>
        /// Retrieves a list of all users available for communication
        /// </summary>
        /// <returns>A list of users with their identity information</returns>
        /// <response code="200">User list retrieved successfully</response>
        /// <response code="500">Internal server error occurred while retrieving users</response>
        [HttpGet("contactable-user")]
        public async Task<IActionResult> GetContactableUsers()
        {
            var users = await _db.GetDataAsync<IdentityDto>("Identity");
            var contactableUsers = users.Where(u => u.Role == "User").ToList();
            return Ok(contactableUsers);
        }

        /// <summary>
        /// Sends a chat message in the system
        /// </summary>
        /// <param name="chat">The chat message data to send</param>
        /// <returns>Success confirmation</returns>
        /// <remarks>
        /// The sender ID is automatically extracted from the JWT token in the Authorization header.
        /// The message is timestamped with the current UTC time and assigned a unique identifier.
        /// </remarks>
        /// <response code="200">Message sent successfully</response>
        /// <response code="400">Invalid message data or missing authorization token</response>
        /// <response code="401">Invalid or expired authorization token</response>
        /// <response code="500">Internal server error occurred while sending the message</response>
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

        /// <summary>
        /// Retrieves all chat messages in the system
        /// </summary>
        /// <returns>A comprehensive list of all chat messages</returns>
        /// <response code="200">Messages retrieved successfully</response>
        /// <response code="500">Internal server error occurred while retrieving messages</response>
        [HttpGet("all-messages")]
        public async Task<IActionResult> GetAllMessages()
        {
            var messages = await _db.GetDataAsync<ChatDto>("Chat");
            return Ok(messages);
        }
    }
}
