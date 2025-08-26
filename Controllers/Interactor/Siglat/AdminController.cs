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
    /// Admin controller for system administration and user management
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
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

        /// <summary>
        /// Get list of all users in the system (Admin only)
        /// </summary>
        /// <returns>List of all users with sanitized data (no passwords)</returns>
        /// <response code="200">Users retrieved successfully</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("userlist")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserList()
        {
            try
            {
                var data = await _db.GetDataAsync<IdentityDto>("Identity");
                
                // Remove sensitive information like passwords
                var sanitizedUsers = data.Select(user => new 
                {
                    user.Id,
                    user.FirstName,
                    user.MiddleName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber,
                    user.Role,
                    user.Gender,
                    user.DateOfBirth,
                    user.Address,
                    user.CreatedAt,
                    user.UpdatedAt
                }).OrderByDescending(u => u.CreatedAt);
                
                return Ok(sanitizedUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve users", error = ex.Message });
            }
        }

        [HttpGet("user/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            try
            {
                var user = await _db.GetSingleDataAsync<IdentityDto>("Identity", id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                
                // Remove sensitive information
                var sanitizedUser = new 
                {
                    user.Id,
                    user.FirstName,
                    user.MiddleName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber,
                    user.Role,
                    user.Gender,
                    user.DateOfBirth,
                    user.Address,
                    user.CreatedAt,
                    user.UpdatedAt
                };
                
                return Ok(sanitizedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve user", error = ex.Message });
            }
        }

        [HttpDelete("user/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                // Check if user exists
                var existingUser = await _db.GetSingleDataAsync<IdentityDto>("Identity", id);
                if (existingUser == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                
                // Prevent deleting the last admin
                if (existingUser.Role == "Admin")
                {
                    var adminCount = (await _db.GetDataByColumnAsync<IdentityDto>("Identity", "Role", "Admin")).Count();
                    if (adminCount <= 1)
                    {
                        return BadRequest(new { message = "Cannot delete the last admin user" });
                    }
                }
                
                await _db.DeleteDataAsync("Identity", id);
                return Ok(new { message = "User deleted successfully", id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete user", error = ex.Message });
            }
        }

        [HttpPost("verification-action")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerificationAction([FromBody] VerificationDto verification)
        {
            try
            {
                verification.UpdatedAt = DateTime.UtcNow;
                await _db.PostDataAsync<VerificationDto>("Verifications", verification, verification.Id);
                return Ok(verification);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update verification", error = ex.Message });
            }
        }


        [HttpPut("user/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] IdentityDto user)
        {
            try
            {
                if (id != user.Id)
                {
                    return BadRequest(new { message = "User ID mismatch" });
                }
                
                var existingUser = await _db.GetSingleDataAsync<IdentityDto>("Identity", id);
                if (existingUser == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                
                // Preserve the password if not provided
                user.HashPass = existingUser.HashPass;
                user.UpdatedAt = DateTime.UtcNow;
                user.CreatedAt = existingUser.CreatedAt; // Preserve original creation date
                
                await _db.PostDataAsync<IdentityDto>("Identity", user, user.Id);
                
                // Return sanitized user data
                var sanitizedUser = new 
                {
                    user.Id,
                    user.FirstName,
                    user.MiddleName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber,
                    user.Role,
                    user.Gender,
                    user.DateOfBirth,
                    user.Address,
                    user.CreatedAt,
                    user.UpdatedAt
                };
                
                return Ok(sanitizedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update user", error = ex.Message });
            }
        }

        [HttpPut("user/{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UserRoleUpdateDto roleUpdate)
        {
            try
            {
                var existingUser = await _db.GetSingleDataAsync<IdentityDto>("Identity", id);
                if (existingUser == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                
                // Prevent removing admin role from the last admin
                if (existingUser.Role == "Admin" && roleUpdate.Role != "Admin")
                {
                    var adminCount = (await _db.GetDataByColumnAsync<IdentityDto>("Identity", "Role", "Admin")).Count();
                    if (adminCount <= 1)
                    {
                        return BadRequest(new { message = "Cannot remove admin role from the last admin user" });
                    }
                }
                
                existingUser.Role = roleUpdate.Role;
                existingUser.UpdatedAt = DateTime.UtcNow;
                
                await _db.PostDataAsync<IdentityDto>("Identity", existingUser, existingUser.Id);
                
                return Ok(new { message = "User role updated successfully", role = roleUpdate.Role });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update user role", error = ex.Message });
            }
        }

        [HttpGet("verifications")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetVerifications()
        {
            try
            {
                var dataTask = _db.GetDataAsync<VerificationDto>("Verifications");
                var dataxTask = _db.GetDataAsync<IdentityDto>("Identity");

                await Task.WhenAll(dataTask, dataxTask);

                var data = await dataTask;
                var datax = await dataxTask;

                var verificationDetails = data.Select(verification => new VerificationDetailsDto
                {
                    Id = verification.Id,
                    B64Image = verification.B64Image,
                    Name = datax.FirstOrDefault(identity => identity.Id == verification.Id)?.FirstName + " " +
                           datax.FirstOrDefault(identity => identity.Id == verification.Id)?.MiddleName + " " +
                           datax.FirstOrDefault(identity => identity.Id == verification.Id)?.LastName,
                    VerificationType = verification.VerificationType,
                    Remarks = verification.Remarks,
                    Status = verification.Status,
                    CreatedAt = verification.CreatedAt,
                    UpdatedAt = verification.UpdatedAt
                }).OrderByDescending(v => v.CreatedAt).ToList();

                return Ok(verificationDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve verifications", error = ex.Message });
            }
        }

        [HttpPut("verification/{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVerificationStatus(Guid id, [FromBody] VerificationStatusUpdateDto statusUpdate)
        {
            try
            {
                if (!VerificationStatus.IsValidStatus(statusUpdate.Status))
                {
                    return BadRequest(new { message = "Invalid verification status" });
                }

                var existingVerification = await _db.GetSingleDataAsync<VerificationDto>("Verifications", id);
                if (existingVerification == null)
                {
                    return NotFound(new { message = "Verification not found" });
                }

                existingVerification.Status = statusUpdate.Status.ToLower();
                existingVerification.Remarks = statusUpdate.Remarks ?? existingVerification.Remarks;
                existingVerification.UpdatedAt = DateTime.UtcNow;

                await _db.PostDataAsync<VerificationDto>("Verifications", existingVerification, existingVerification.Id);

                return Ok(new { message = "Verification status updated successfully", status = existingVerification.Status });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update verification status", error = ex.Message });
            }
        }

        [HttpGet("contact")]
        [AllowAnonymous]
        public async Task<IActionResult> Contacts()
        {
            var data = await _db.GetDataAsync<ContactDto>("Contact");
            data = data.OrderBy(x => x.ContactType).ToList();
            return Ok(data);
        }

        [HttpDelete("contact")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteContact(Guid Id)
        {
            await _db.DeleteDataAsync("Contact", Id);
            return Ok(Id);
        }

        [HttpPost("contact")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Contact([FromBody] ContactDto Contact)
        {
            Contact.CreatedAt = DateTime.UtcNow;
            Contact.UpdatedAt = DateTime.UtcNow;
            await _db.PostDataAsync<ContactDto>("Contact", Contact, Contact.Id);
            return Ok(Contact);
        }

        [HttpPut("contact")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateContact([FromBody] ContactDto Contact)
        {
            Contact.UpdatedAt = DateTime.UtcNow;
            await _db.PostDataAsync<ContactDto>("Contact", Contact, Contact.Id);
            return Ok(Contact);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Get the current user ID from the JWT token
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value ?? 
                                 User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                                 User.FindFirst("nameid")?.Value ?? 
                                 User.FindFirst("sub")?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return BadRequest(new { message = "Invalid user ID in token" });
                }

                var user = await _db.GetSingleDataAsync<IdentityDto>("Identity", userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Return sanitized profile data
                var profile = new 
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber,
                    user.Role,
                    Department = "Emergency Response System", // Static for now
                    Location = "Villaverde, Nueva Vizcaya"     // Static for now
                };

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve profile", error = ex.Message });
            }
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileDto profileData)
        {
            try
            {
                // Get the current user ID from the JWT token
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value ?? 
                                 User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                                 User.FindFirst("nameid")?.Value ?? 
                                 User.FindFirst("sub")?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return BadRequest(new { message = "Invalid user ID in token" });
                }

                var existingUser = await _db.GetSingleDataAsync<IdentityDto>("Identity", userId);
                if (existingUser == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Update only the profile fields that are allowed to be changed
                existingUser.FirstName = profileData.FirstName;
                existingUser.LastName = profileData.LastName;
                existingUser.Email = profileData.Email;
                existingUser.PhoneNumber = profileData.PhoneNumber;
                existingUser.UpdatedAt = DateTime.UtcNow;

                // Save the updated user
                await _db.PostDataAsync<IdentityDto>("Identity", existingUser, existingUser.Id);

                // Return updated profile data
                var updatedProfile = new 
                {
                    existingUser.Id,
                    existingUser.FirstName,
                    existingUser.LastName,
                    existingUser.Email,
                    existingUser.PhoneNumber,
                    existingUser.Role,
                    Department = profileData.Department ?? "Emergency Response System",
                    Location = profileData.Location ?? "Villaverde, Nueva Vizcaya"
                };

                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update profile", error = ex.Message });
            }
        }
    }
}
