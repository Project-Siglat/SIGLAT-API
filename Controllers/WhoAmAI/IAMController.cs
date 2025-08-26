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
    /// <summary>
    /// Identity and Access Management (IAM) controller for user profile and verification management
    /// </summary>
    /// <remarks>
    /// This controller handles user identity operations including profile retrieval, updates,
    /// password changes, and verification processes for the authenticated user.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IAMController : ControllerBase
    {
        private readonly IPostgreService _db;
        private readonly IHttpClientFactory _httpClientFactory;

        public IAMController(IPostgreService db, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;

        }
        /// <summary>
        /// Retrieves the current user's identity information
        /// </summary>
        /// <returns>The authenticated user's complete identity data</returns>
        /// <remarks>
        /// Extracts the user ID from the JWT token and retrieves the corresponding user data.
        /// </remarks>
        /// <response code="200">User identity retrieved successfully</response>
        /// <response code="401">Unauthorized - valid authentication token required</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error occurred while retrieving user data</response>
        [HttpGet]
        public async Task<IActionResult> IAM()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            var tokenData = jsonToken.Payload.Jti;

            // var whos = await _db.GetDataAsync<IdentityDto>("Identity");
            var whoami = await _db.GetSingleDataAsync<IdentityDto>("Identity", tokenData.ToString());
            return Ok(whoami);
            // return Ok(tokenData.ToString());
        }

        /// <summary>
        /// Updates user identity information directly
        /// </summary>
        /// <param name="identityDto">The complete identity data to update</param>
        /// <returns>The updated identity information</returns>
        /// <remarks>
        /// This endpoint allows direct update of user identity data with the provided DTO.
        /// Use the /update endpoint for safer, token-validated updates.
        /// </remarks>
        /// <response code="200">Identity updated successfully</response>
        /// <response code="400">Invalid identity data</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error occurred while updating identity</response>
        [HttpPost]
        public async Task<IActionResult> UpdateIAM([FromBody] IdentityDto identityDto)
        {
            // var whos = await _db.GetDataAsync<IdentityDto>("Identity");
            var whoami = await _db.PostDataAsync<IdentityDto>("Identity", identityDto, identityDto.Id);
            return Ok(whoami);
            // return Ok(tokenData.ToString());
        }

        /// <summary>
        /// Updates the authenticated user's profile information
        /// </summary>
        /// <param name="identityDto">The profile data to update</param>
        /// <returns>The updated user profile</returns>
        /// <remarks>
        /// Updates the current user's profile information based on the JWT token.
        /// The user ID is extracted from the token to ensure secure updates.
        /// The UpdatedAt timestamp is automatically set to the current UTC time.
        /// </remarks>
        /// <response code="200">Profile updated successfully</response>
        /// <response code="400">Invalid profile data</response>
        /// <response code="401">Unauthorized - valid authentication token required</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error occurred while updating profile</response>
        [HttpPost("update")]
        public async Task<IActionResult> ChangeInfo([FromBody] IdentityDto identityDto)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            var tokenData = jsonToken.Payload.Jti;

            var user = await _db.GetSingleDataAsync<IdentityDto>("Identity", tokenData.ToString());
            user.Id = Guid.Parse(tokenData);
            user.FirstName = identityDto.FirstName;
            user.MiddleName = identityDto.MiddleName;
            user.LastName = identityDto.LastName;
            user.Address = identityDto.Address;
            user.Gender = identityDto.Gender;
            user.PhoneNumber = identityDto.PhoneNumber;
            user.Role = identityDto.Role;
            user.DateOfBirth = identityDto.DateOfBirth;
            user.Email = identityDto.Email;
            user.UpdatedAt = DateTime.UtcNow;


            var whoami = await _db.PostDataAsync<IdentityDto>("Identity", user, user.Id);
            return Ok(whoami);
        }

        /// <summary>
        /// Changes the authenticated user's password
        /// </summary>
        /// <param name="pass">The new password to set</param>
        /// <returns>Updated user identity with new password hash</returns>
        /// <remarks>
        /// Updates the password for the authenticated user. The password is automatically hashed
        /// using the system's password hashing service before storage.
        /// </remarks>
        /// <response code="200">Password changed successfully</response>
        /// <response code="400">Invalid password data</response>
        /// <response code="401">Unauthorized - valid authentication token required</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error occurred while changing password</response>
        [HttpPost("change-pass")]
        public async Task<IActionResult> ChangePassword(string pass)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            var tokenData = jsonToken.Payload.Jti;

            var identity = await _db.GetSingleDataAsync<IdentityDto>("Identity", tokenData.ToString());
            identity.HashPass = PasswordService.HashPassword(pass);
            await _db.PostDataAsync<IdentityDto>("Identity", identity, identity.Id);
            return Ok(identity);
        }

        /// <summary>
        /// Submits user verification documents
        /// </summary>
        /// <param name="image">The verification document image file</param>
        /// <param name="DocuType">The type of document being submitted for verification</param>
        /// <returns>Verification submission confirmation</returns>
        /// <remarks>
        /// Allows users to submit verification documents (images) for account verification.
        /// The image is converted to base64 and stored with the verification request.
        /// Requires complete user profile information before verification can be submitted.
        /// </remarks>
        /// <response code="200">Verification document submitted successfully</response>
        /// <response code="400">User profile incomplete or invalid document</response>
        /// <response code="401">Unauthorized - valid authentication token required</response>
        /// <response code="500">Internal server error occurred while processing verification</response>
        [HttpPost("verify")]
        public async Task<IActionResult> Verify(IFormFile image, string DocuType)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            var tokenData = jsonToken.Payload.Jti;

            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                var IMG = Convert.ToBase64String(imageBytes);

                var verification = new VerificationDto
                {
                    Id = Guid.Parse(tokenData),
                    B64Image = IMG,
                    Status = "pending",
                    Remarks = "",
                    VerificationType = DocuType,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var me = await _db.GetSingleDataAsync<IdentityDto>("Identity", tokenData);
                if (me == null ||
                                    me.Id == Guid.Empty ||
                                    string.IsNullOrEmpty(me.FirstName) ||
                                    string.IsNullOrEmpty(me.MiddleName) ||
                                    string.IsNullOrEmpty(me.LastName) ||
                                    string.IsNullOrEmpty(me.Address) ||
                                    string.IsNullOrEmpty(me.Gender) ||
                                    string.IsNullOrEmpty(me.PhoneNumber) ||
                                    string.IsNullOrEmpty(me.Role) ||
                                    me.DateOfBirth == DateTime.MinValue ||
                                    string.IsNullOrEmpty(me.Email) ||
                                    string.IsNullOrEmpty(me.HashPass) ||
                                    me.CreatedAt == DateTime.MinValue ||
                                    me.UpdatedAt == DateTime.MinValue)
                {
                    return BadRequest("User data is incomplete. Please complete your profile before verification.");
                }

                await _db.PostDataAsync<VerificationDto>("Verifications", verification, verification.Id);
                return Ok("Verify?");
            }
        }


        /// <summary>
        /// Checks the verification status of the authenticated user
        /// </summary>
        /// <returns>The current verification status of the user</returns>
        /// <remarks>
        /// Returns the verification status which can be:
        /// - "accepted" - verification approved
        /// - "pending" - verification under review
        /// - "rejected" - verification denied
        /// - "none" - no verification submitted
        /// </remarks>
        /// <response code="200">Verification status retrieved successfully</response>
        /// <response code="401">Unauthorized - valid authentication token required</response>
        /// <response code="500">Internal server error occurred while checking verification status</response>
        [HttpGet("verified")]
        public async Task<IActionResult> IsVerified()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            var tokenData = jsonToken.Payload.Jti;

            var verified = await _db.GetSingleDataAsync<VerificationDto>("Verifications", tokenData.ToString());
            // return Ok(verified.Status);
            if (verified != null)
            {
                if (verified.Status == "approved")
                {
                    return Ok("accepted");

                }
                else if (verified.Status == "pending")
                {
                    return Ok("pending");
                }
                else
                {
                    return Ok("rejected");
                }
            }
            else
            {
                return Ok("none");
            }
        }
    }
}
