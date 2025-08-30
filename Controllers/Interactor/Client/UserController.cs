using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Rai.SIGLAT.API.Models;
using Rai.SIGLAT.API.Services;

namespace Rai.SIGLAT.API.Controllers.Client
{
    /// <summary>
    /// Controller for managing user-specific operations and location tracking
    /// </summary>
    /// <remarks>
    /// This controller handles user-related operations including location coordinate updates
    /// for emergency response tracking and positioning services.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IPostgreService _db;
        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(IPostgreService db, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;

        }
        /// <summary>
        /// Updates the geographical coordinates of the authenticated user
        /// </summary>
        /// <param name="user">The user coordinate data containing latitude and longitude information</param>
        /// <returns>Success confirmation message</returns>
        /// <remarks>
        /// This endpoint allows users to update their current location coordinates for emergency response tracking.
        /// The user ID is automatically extracted from the JWT token to ensure secure location updates.
        /// </remarks>
        /// <response code="200">Coordinates updated successfully</response>
        /// <response code="400">Invalid coordinate data or missing authorization token</response>
        /// <response code="401">Unauthorized - valid authentication token required</response>
        /// <response code="500">Internal server error occurred while updating coordinates</response>
        [HttpPost("coordinates")]
        // [AllowAnonymous]
        public async Task<IActionResult> Get([FromBody] UserXYZDto user)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            var tokenData = jsonToken.Payload.Jti;

            user.Id = Guid.Parse(tokenData);
            await _db.PostDataAsync<UserXYZDto>("UserXYZ", user, user.Id);
            return Ok("Success");
        }
    }
}
