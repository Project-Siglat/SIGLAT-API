using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SIGLAT.API.Model;
using Craftmatrix.org.Model;
namespace SIGLAT.API.Controllers.Ambulance
{
    /// <summary>
    /// Controller for ambulance service management and emergency medical response coordination
    /// </summary>
    /// <remarks>
    /// This controller manages ambulance operations including alert handling, unit tracking,
    /// emergency response coordination, and ambulance availability management within the SIGLAT system.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    // [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AmbulanceController : ControllerBase
    {
        private readonly IPostgreService _db;
        private readonly IHttpClientFactory _httpClientFactory;

        public AmbulanceController(IPostgreService db, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Retrieves all emergency alerts ordered by response time
        /// </summary>
        /// <returns>A list of all emergency alerts sorted by most recent response time</returns>
        /// <response code="200">Alerts retrieved successfully</response>
        /// <response code="500">Internal server error occurred while retrieving alerts</response>
        [HttpGet("all-alert")]
        public async Task<IActionResult> GetAllAlerts()
        {
            var data = await _db.GetDataAsync<AlertDto>("Alerts");
            var latest = data.OrderByDescending(x => x.RespondedAt).ToArray();
            return Ok(latest);
        }

        /// <summary>
        /// Creates a new emergency alert
        /// </summary>
        /// <param name="alerto">The alert data to create</param>
        /// <returns>Success confirmation message</returns>
        /// <remarks>
        /// The user ID is automatically extracted from the JWT token in the Authorization header.
        /// The alert is timestamped with the current UTC time and assigned a unique identifier.
        /// </remarks>
        /// <response code="200">Alert posted successfully</response>
        /// <response code="400">Invalid alert data or missing authorization token</response>
        /// <response code="401">Invalid or expired authorization token</response>
        /// <response code="500">Internal server error occurred while creating the alert</response>
        [HttpPost("alert")]
        [AllowAnonymous]
        public async Task<IActionResult> Alert([FromBody] AlertDto alerto)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            var tokenData = jsonToken.Payload.Jti;

            alerto.Id = Guid.NewGuid();
            alerto.Uid = Guid.Parse(tokenData);
            alerto.RespondedAt = DateTime.UtcNow;
            await _db.PostDataAsync<AlertDto>("Alerts", alerto, alerto.Id);
            return Ok("Alert posted successfully");
        }

        /// <summary>
        /// Retrieves the most recent emergency alert
        /// </summary>
        /// <returns>The latest emergency alert based on response time</returns>
        /// <response code="200">Latest alert retrieved successfully</response>
        /// <response code="204">No alerts found</response>
        /// <response code="500">Internal server error occurred while retrieving the alert</response>
        [HttpGet("alert")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAlert()
        {
            var data = await _db.GetDataAsync<AlertDto>("Alerts");
            var latest = data.OrderByDescending(x => x.RespondedAt).ToArray();
            return Ok(latest.FirstOrDefault());
        }

        /// <summary>
        /// Retrieves the current emergency alert for the authenticated user
        /// </summary>
        /// <returns>The current alert for the requesting user with responder location data</returns>
        /// <remarks>
        /// Returns the latest alert associated with the authenticated user, including the responder's
        /// geographical coordinates. Returns 204 No Content if the alert status is "Done".
        /// </remarks>
        /// <response code="200">Current alert retrieved successfully</response>
        /// <response code="204">Alert is completed (status: Done)</response>
        /// <response code="404">Responder location not found</response>
        /// <response code="401">Invalid or expired authorization token</response>
        /// <response code="500">Internal server error occurred while retrieving the alert</response>
        [HttpGet("alert/current")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCurrentAlert()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            var tokenData = jsonToken.Payload.Jti;

            var data = await _db.GetDataAsync<AlertDto>("Alerts");
            var latest = data.OrderByDescending(x => x.RespondedAt).ToArray();
            var specific = latest.FirstOrDefault(x => x.Uid == Guid.Parse(tokenData));

            var zawarudo = await _db.GetSingleDataAsync<UserXYZDto>("UserXYZ", specific.Responder);
            if (zawarudo == null)
            {
                return NotFound("Responder not found");
            }
            specific.Latitude = zawarudo.Latitude;
            specific.Longitude = zawarudo.Longitude;

            // Don't update route if status is done
            if (specific.Status == "Done")
            {
                return NoContent();
            }

            return Ok(specific);
        }

        /// <summary>
        /// Retrieves the current emergency alert assigned to the authenticated ambulance personnel
        /// </summary>
        /// <returns>The current alert assigned to the ambulance with patient location data</returns>
        /// <remarks>
        /// Returns the latest alert where the authenticated user is assigned as the responder,
        /// including the patient's geographical coordinates. Returns 204 No Content if the alert status is "Done".
        /// </remarks>
        /// <response code="200">Current ambulance alert retrieved successfully</response>
        /// <response code="204">Alert is completed (status: Done)</response>
        /// <response code="404">Patient location not found</response>
        /// <response code="401">Invalid or expired authorization token</response>
        /// <response code="500">Internal server error occurred while retrieving the alert</response>
        [HttpGet("alert/current/amb")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCurrentAlertAmb()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            var tokenData = jsonToken.Payload.Jti;


            var data = await _db.GetDataAsync<AlertDto>("Alerts");
            var latest = data.OrderByDescending(x => x.RespondedAt).ToArray();
            var specific = latest.FirstOrDefault(x => x.Responder == Guid.Parse(tokenData));

            // return Ok(new { tokenData, latest });

            var zawarudo = await _db.GetSingleDataAsync<UserXYZDto>("UserXYZ", specific.Uid);
            if (zawarudo == null)
            {
                return NotFound("Responder not found");
            }
            specific.Latitude = zawarudo.Latitude;
            specific.Longitude = zawarudo.Longitude;

            if (specific.Status == "Done")
            {
                return NoContent();
            }

            return Ok(specific);
        }

        /// <summary>
        /// Marks an ambulance emergency response as completed
        /// </summary>
        /// <param name="id">The unique identifier of the ambulance personnel completing the response</param>
        /// <returns>The updated alert with "Done" status</returns>
        /// <remarks>
        /// Updates the status of the emergency alert to "Done" for the specified ambulance responder.
        /// This indicates that the emergency response has been completed.
        /// </remarks>
        /// <response code="200">Alert marked as completed successfully</response>
        /// <response code="404">Alert not found for the specified ambulance ID</response>
        /// <response code="500">Internal server error occurred while updating the alert status</response>
        [HttpGet("alert/current/amb/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> AmbulanceDone(Guid id)
        {
            var data = await _db.GetDataAsync<AlertDto>("Alerts");
            var latest = data.OrderByDescending(x => x.RespondedAt).ToArray();
            var specific = latest.FirstOrDefault(x => x.Responder == id);

            if (specific == null)
            {
                return NotFound("Alert not found");
            }

            specific.Status = "Done";
            await _db.PostDataAsync<AlertDto>("Alerts", specific, specific.Id);

            return Ok(specific);
        }

        /// <summary>
        /// Retrieves a list of all ambulance personnel with their current locations
        /// </summary>
        /// <returns>A list of ambulance personnel with their geographical coordinates</returns>
        /// <remarks>
        /// Returns all users with "Ambulance" role along with their current location coordinates
        /// from the UserXYZ table. Only includes ambulances that have location data available.
        /// </remarks>
        /// <response code="200">Ambulance list with locations retrieved successfully</response>
        /// <response code="500">Internal server error occurred while retrieving ambulance data</response>
        [HttpGet]
        public async Task<IActionResult> AmbulanceLists()
        {
            var data = await _db.GetDataAsync<IdentityDto>("Identity");
            var ambulanceOnly = data.Where(r => r.Role.ToUpper() == "ambulance".ToUpper());

            var coordinates = ambulanceOnly.Select(e => e.Id).ToArray();

            var xyList = new List<UserXYZDto>();
            for (int i = 0; i < coordinates.Count(); i++)
            {
                var xy = await _db.GetSingleDataAsync<UserXYZDto>("UserXYZ", coordinates[i]);
                if (xy != null)
                {
                    xyList.Add(xy);
                }
            }
            return Ok(xyList);
        }
    }
}
