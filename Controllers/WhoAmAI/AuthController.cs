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
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IPostgreService _db;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IPostgreService db, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;

        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            try
            {
                var existingIdentity = await _db.GetDataByColumnAsync<IdentityDto>("Identity", "Email", request.Email);
                if (existingIdentity.Count() > 0)
                {
                    throw new Exception("Email already exists");
                }

                var identity = new IdentityDto
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName,
                    Address = request.Address,
                    Gender = request.Gender,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    Email = request.Email,
                    Role = "User", // Set default role
                    HashPass = PasswordService.HashPassword(request.HashPass),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Create a new object with DateOfBirth converted to DateTime to avoid Dapper DateOnly issues
                var identityForDb = new
                {
                    identity.Id,
                    identity.FirstName,
                    identity.MiddleName,
                    identity.LastName,
                    identity.Address,
                    identity.Role,
                    identity.DateOfBirth,
                    identity.Gender,
                    identity.PhoneNumber,
                    identity.Email,
                    identity.HashPass,
                    identity.CreatedAt,
                    identity.UpdatedAt
                };

                await _db.PostDataAsync("Identity", identityForDb, identity.Id);
                return Ok("Registration successful");
            }
            catch (Exception ex)
            {
                return BadRequest($"Registration failed: {ex.Message}");
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AuthDto request)
        {
            var existingIdentity = await _db.GetDataByColumnAsync<IdentityDto>("Identity", "Email", request.Email);
            var data = existingIdentity.FirstOrDefault();
            if (data == null)
            {
                return NotFound("User not found");
            }
            else
            {
                var verify = PasswordService.VerifyPassword(request.Password, data.HashPass);
                if (verify)
                {
                    var token = GenerateToken(data.Email, data.Id.ToString(), data.Role);
                    LoginLogsDto logogo = new LoginLogsDto
                    {
                        Id = Guid.NewGuid(),
                        Who = data.Id,
                        What = "Success",
                        CreatedAt = DateTime.UtcNow,
                    };
                    await _db.PostDataAsync<LoginLogsDto>("LoginLogs", logogo, logogo.Id);
                    return Ok(new { role = data.Role, token });

                }
                else
                {
                    LoginLogsDto logogo = new LoginLogsDto
                    {
                        Id = Guid.NewGuid(),
                        Who = data.Id,
                        What = "Failed",
                        CreatedAt = DateTime.UtcNow,
                    };
                    await _db.PostDataAsync<LoginLogsDto>("LoginLogs", logogo, logogo.Id);
                    return BadRequest("Wrong Password");
                }
                // return Ok(new { pass, data.HashPass });

            }
        }

        private string GenerateToken(string email, string userId, string role)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrEmpty(role)) throw new ArgumentNullException(nameof(role));

            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new InvalidOperationException("JWT_SECRET environment variable is not set.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                                    new Claim(JwtRegisteredClaimNames.Sub, email),
                                    new Claim(JwtRegisteredClaimNames.Jti, userId),
                                    new Claim(ClaimTypes.Role, role)
                                }),
                Expires = DateTime.UtcNow.AddMonths(1),
                Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var jtiClaim = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(jtiClaim))
                {
                    return Unauthorized("Invalid token");
                }

                if (!Guid.TryParse(jtiClaim, out Guid userId))
                {
                    return Unauthorized("Invalid user ID in token");
                }

                var userProfile = await _db.GetDataByColumnAsync<IdentityDto>("Identity", "Id", userId);
                var user = userProfile.FirstOrDefault();
                
                if (user == null)
                {
                    return NotFound("User not found");
                }

                // Return user profile without sensitive data
                var profileResponse = new
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    Gender = user.Gender,
                    DateOfBirth = user.DateOfBirth,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return Ok(profileResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving profile: {ex.Message}");
            }
        }
    }
}
