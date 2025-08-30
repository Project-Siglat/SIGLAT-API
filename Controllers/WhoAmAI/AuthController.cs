using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Craftmatrix.org.API.Models;
using Craftmatrix.org.API.Services;
using Craftmatrix.org.API.Services;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Craftmatrix.org.API.Controllers.Authentication
{
    /// <summary>
    /// Authentication controller for user registration, login, and profile management
    /// </summary>
    /// <remarks>
    /// This controller handles user authentication processes including new user registration,
    /// login with JWT token generation, and authenticated profile retrieval. It also manages
    /// login logging for security auditing purposes.
    /// </remarks>
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

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="request">User registration details including personal information and credentials</param>
        /// <returns>Success message or error details</returns>
        /// <response code="200">Registration successful</response>
        /// <response code="400">Registration failed - email already exists or validation error</response>
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
                    RoleId = 2, // Set default role to "User"
                    IsEmailVerified = false,
                    IsPhoneVerified = false,
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
                    identity.RoleId,
                    identity.DateOfBirth,
                    identity.Gender,
                    identity.PhoneNumber,
                    identity.Email,
                    identity.IsEmailVerified,
                    identity.IsPhoneVerified,
                    EmailVerifiedAt = (DateTime?)null,
                    PhoneVerifiedAt = (DateTime?)null,
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

        /// <summary>
        /// Authenticate user and obtain access token
        /// </summary>
        /// <param name="request">User email and password credentials</param>
        /// <returns>JWT token and user role information</returns>
        /// <response code="200">Login successful - returns JWT token and user role</response>
        /// <response code="400">Invalid credentials</response>
        /// <response code="404">User not found</response>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AuthDto request)
        {
            UserLoginTrackingDto loginTracking;
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            try
            {
                var existingIdentity = await _db.GetDataByColumnAsync<IdentityDto>("Identity", "Email", request.Email);
                var data = existingIdentity.FirstOrDefault();

                if (data == null)
                {
                    // Log failed login attempt for non-existent user
                    loginTracking = new UserLoginTrackingDto
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.Empty, // No user ID for non-existent user
                        IpAddress = ipAddress,
                        UserAgent = userAgent,
                        LoginTimestamp = DateTime.UtcNow,
                        LoginStatus = "Failed",
                        FailureReason = "User not found",
                        AttemptedEmail = request.Email
                    };
                    await _db.PostDataAsync<UserLoginTrackingDto>("UserLoginTracking", loginTracking, loginTracking.Id);

                    return NotFound("User not found");
                }

                var verify = PasswordService.VerifyPassword(request.Password, data.HashPass);
                if (verify)
                {
                    var token = GenerateToken(data.Email, data.Id.ToString(), data.RoleId.ToString());

                    loginTracking = new UserLoginTrackingDto
                    {
                        Id = Guid.NewGuid(),
                        UserId = data.Id,
                        IpAddress = ipAddress,
                        UserAgent = userAgent,
                        LoginTimestamp = DateTime.UtcNow,
                        LoginStatus = "Success"
                    };
                    await _db.PostDataAsync<UserLoginTrackingDto>("UserLoginTracking", loginTracking, loginTracking.Id);

                    return Ok(new { roleId = data.RoleId, token, userId = data.Id });
                }
                else
                {
                    loginTracking = new UserLoginTrackingDto
                    {
                        Id = Guid.NewGuid(),
                        UserId = data.Id,
                        IpAddress = ipAddress,
                        UserAgent = userAgent,
                        LoginTimestamp = DateTime.UtcNow,
                        LoginStatus = "Failed",
                        FailureReason = "Wrong Password"
                    };
                    await _db.PostDataAsync<UserLoginTrackingDto>("UserLoginTracking", loginTracking, loginTracking.Id);

                    return BadRequest("Wrong Password");
                }
            }
            catch (Exception ex)
            {
                // Log system error
                loginTracking = new UserLoginTrackingDto
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Empty,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    LoginTimestamp = DateTime.UtcNow,
                    LoginStatus = "Error",
                    FailureReason = $"System error: {ex.Message}",
                    AttemptedEmail = request.Email
                };
                await _db.PostDataAsync<UserLoginTrackingDto>("UserLoginTracking", loginTracking, loginTracking.Id);

                return StatusCode(500, "Login system error");
            }
        }

        private string GenerateToken(string email, string userId, string roleId)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrEmpty(roleId)) throw new ArgumentNullException(nameof(roleId));

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
                                    new Claim(ClaimTypes.Role, roleId)
                                }),
                Expires = DateTime.UtcNow.AddMonths(1),
                Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Check if any admin account exists in the system
        /// </summary>
        /// <returns>Boolean indicating if admin exists</returns>
        /// <response code="200">Check successful</response>
        [HttpGet("admin-exists")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckAdminExists()
        {
            try
            {
                var adminUsers = await _db.GetDataByColumnAsync<IdentityDto>("Identity", "RoleId", 1); // RoleId 1 = Admin
                var exists = adminUsers.Any();

                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error checking admin existence: {ex.Message}");
            }
        }

        /// <summary>
        /// Create the first admin account
        /// </summary>
        /// <param name="request">Admin credentials</param>
        /// <returns>Success message</returns>
        /// <response code="200">Admin created successfully</response>
        /// <response code="400">Admin already exists or validation error</response>
        [HttpPost("create-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAdmin([FromBody] AuthDto request)
        {


            // Che/ck if admin already exists
            var existingAdmins = await _db.GetDataByColumnAsync<IdentityDto>("Identity", "RoleId", 1);
            if (existingAdmins.Any())
            {
                return BadRequest("Admin account already exists");
            }

            // Validate email format
            if (string.IsNullOrEmpty(request.Email) || !IsValidEmail(request.Email))
            {
                return BadRequest("Valid email address is required");
            }

            // Validate password strength
            if (string.IsNullOrEmpty(request.Password) || !IsValidPassword(request.Password))
            {
                return BadRequest("Password must be at least 12 characters with uppercase, lowercase, numbers, and special characters");
            }

            // Check if email already exists
            var existingIdentity = await _db.GetDataByColumnAsync<IdentityDto>("Identity", "Email", request.Email);
            if (existingIdentity.Any())
            {
                return BadRequest("Email already exists");
            }

            var roles = await _db.GetDataByColumnAsync<RoleDto>("Roles", "Name", "Admin");

            var therole = roles.FirstOrDefault().Id;


            var adminId = Guid.NewGuid();
            var adminForDb = new
            {
                Id = adminId,
                FirstName = "Admin",
                MiddleName = "",
                LastName = "User",
                Address = "",
                Gender = "Other",
                PhoneNumber = "",
                DateOfBirth = DateTime.UtcNow.AddYears(-30),
                Email = request.Email,
                RoleId = therole,
                IsEmailVerified = true,
                IsPhoneVerified = false,
                EmailVerifiedAt = (DateTime?)DateTime.UtcNow,
                PhoneVerifiedAt = (DateTime?)null,
                HashPass = PasswordService.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            //return Ok(adminForDb);

            await _db.PostDataAsync("Identity", adminForDb, adminId);

            // return Ok("test?");

            // Log the admin creation
            var loginTracking = new UserLoginTrackingDto
            {
                Id = Guid.NewGuid(),
                UserId = adminId,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                LoginTimestamp = DateTime.UtcNow,
                LoginStatus = "Admin Created"
            };

            // return Ok(loginTracking);
            await _db.PostDataAsync<UserLoginTrackingDto>("UserLoginTracking", loginTracking, loginTracking.Id);

            return Ok(new { message = "Admin account created successfully", adminId = adminId });
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 12)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        /// <summary>
        /// Get login tracking logs (Admin only)
        /// </summary>
        /// <returns>Login tracking data</returns>
        [HttpGet("login-logs")]
        [Authorize(Roles = "1")] // Admin role only
        public async Task<IActionResult> GetLoginLogs()
        {
            try
            {
                var logs = await _db.GetDataAsync<UserLoginTrackingDto>("UserLoginTracking");
                var sortedLogs = logs.OrderByDescending(l => l.LoginTimestamp).Take(100); // Last 100 entries

                return Ok(sortedLogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving login logs: {ex.Message}");
            }
        }

        /// <summary>
        /// Get current authenticated user's profile information
        /// </summary>
        /// <returns>User profile data without sensitive information</returns>
        /// <response code="200">Profile retrieved successfully</response>
        /// <response code="401">Invalid or missing authentication token</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
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
                    RoleId = user.RoleId,
                    IsEmailVerified = user.IsEmailVerified,
                    EmailVerifiedAt = user.EmailVerifiedAt,
                    IsPhoneVerified = user.IsPhoneVerified,
                    PhoneVerifiedAt = user.PhoneVerifiedAt,
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

        /// <summary>
        /// Send verification code to email or phone number
        /// </summary>
        /// <param name="request">Contact verification request</param>
        /// <returns>Success message with expiration time</returns>
        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] SendVerificationCodeDto request)
        {
            try
            {
                // Get current user ID from the token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                // Validate verification type
                if (!ContactVerificationType.IsValidType(request.VerificationType))
                {
                    return BadRequest(new { message = "Invalid verification type. Must be 'email' or 'phone'" });
                }

                // Get user to verify the contact matches their profile
                var user = await _db.GetSingleDataAsync<IdentityDto>("Identity", userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Verify the contact value matches user's profile
                bool contactMatches = request.VerificationType.ToLower() switch
                {
                    "email" => user.Email.Equals(request.ContactValue, StringComparison.OrdinalIgnoreCase),
                    "phone" => user.PhoneNumber.Equals(request.ContactValue),
                    _ => false
                };

                if (!contactMatches)
                {
                    return BadRequest(new { message = "Contact value does not match your profile" });
                }

                // Check if already verified
                bool isAlreadyVerified = request.VerificationType.ToLower() switch
                {
                    "email" => user.IsEmailVerified,
                    "phone" => user.IsPhoneVerified,
                    _ => false
                };

                if (isAlreadyVerified)
                {
                    return BadRequest(new { message = $"{request.VerificationType} is already verified" });
                }

                // Check for recent unused tokens
                var recentTokens = await _db.GetDataAsync<ContactVerificationTokenDto>("ContactVerificationTokens");
                var existingToken = recentTokens.FirstOrDefault(t =>
                    t.UserId == userId &&
                    t.VerificationType == request.VerificationType.ToLower() &&
                    !t.IsUsed &&
                    t.ExpiresAt > DateTime.UtcNow &&
                    t.CreatedAt > DateTime.UtcNow.AddMinutes(-2)); // Don't allow new codes within 2 minutes

                if (existingToken != null)
                {
                    var remainingTime = existingToken.ExpiresAt.Subtract(DateTime.UtcNow);
                    return BadRequest(new
                    {
                        message = "A verification code was recently sent. Please wait before requesting a new one.",
                        retryAfter = remainingTime.TotalSeconds
                    });
                }

                // Generate 6-digit verification code
                var random = new Random();
                var verificationCode = random.Next(100000, 999999).ToString();

                // Create verification token
                var token = new ContactVerificationTokenDto
                {
                    UserId = userId,
                    VerificationType = request.VerificationType.ToLower(),
                    ContactValue = request.ContactValue,
                    VerificationCode = verificationCode,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10), // 10 minutes expiration
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
                };

                await _db.PostDataAsync<ContactVerificationTokenDto>("ContactVerificationTokens", token, token.Id);

                // TODO: In a real implementation, send the code via email or SMS
                // For development, we'll return the code in the response
                return Ok(new
                {
                    message = $"Verification code sent to your {request.VerificationType}",
                    expiresAt = token.ExpiresAt,
                    // Remove this in production - only for development
                    verificationCode = verificationCode
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to send verification code", error = ex.Message });
            }
        }

        /// <summary>
        /// Verify email or phone number with verification code
        /// </summary>
        /// <param name="request">Verification code request</param>
        /// <returns>Verification success message</returns>
        [HttpPost("verify-contact")]
        public async Task<IActionResult> VerifyContact([FromBody] VerifyContactCodeDto request)
        {
            try
            {
                // Get current user ID from the token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                // Validate verification type
                if (!ContactVerificationType.IsValidType(request.VerificationType))
                {
                    return BadRequest(new { message = "Invalid verification type. Must be 'email' or 'phone'" });
                }

                // Find the verification token
                var tokens = await _db.GetDataAsync<ContactVerificationTokenDto>("ContactVerificationTokens");
                var token = tokens.FirstOrDefault(t =>
                    t.UserId == userId &&
                    t.VerificationType == request.VerificationType.ToLower() &&
                    t.ContactValue == request.ContactValue &&
                    !t.IsUsed &&
                    t.ExpiresAt > DateTime.UtcNow);

                if (token == null)
                {
                    return BadRequest(new { message = "Invalid or expired verification code" });
                }

                // Check attempt count to prevent brute force
                if (token.AttemptCount >= 5)
                {
                    return BadRequest(new { message = "Too many verification attempts. Please request a new code." });
                }

                // Verify the code
                if (token.VerificationCode != request.VerificationCode)
                {
                    // Increment attempt count
                    token.AttemptCount++;
                    await _db.PostDataAsync<ContactVerificationTokenDto>("ContactVerificationTokens", token, token.Id);

                    return BadRequest(new
                    {
                        message = "Invalid verification code",
                        attemptsRemaining = Math.Max(0, 5 - token.AttemptCount)
                    });
                }

                // Mark token as used
                token.IsUsed = true;
                token.VerifiedAt = DateTime.UtcNow;
                await _db.PostDataAsync<ContactVerificationTokenDto>("ContactVerificationTokens", token, token.Id);

                // Update user verification status
                var user = await _db.GetSingleDataAsync<IdentityDto>("Identity", userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                if (request.VerificationType.ToLower() == "email")
                {
                    user.IsEmailVerified = true;
                    user.EmailVerifiedAt = DateTime.UtcNow;
                }
                else if (request.VerificationType.ToLower() == "phone")
                {
                    user.IsPhoneVerified = true;
                    user.PhoneVerifiedAt = DateTime.UtcNow;
                }

                user.UpdatedAt = DateTime.UtcNow;
                await _db.PostDataAsync<IdentityDto>("Identity", user, user.Id);

                return Ok(new
                {
                    message = $"{request.VerificationType} verified successfully",
                    verifiedAt = DateTime.UtcNow,
                    verificationType = request.VerificationType
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to verify contact", error = ex.Message });
            }
        }

        /// <summary>
        /// Get verification status for the current user
        /// </summary>
        /// <returns>Verification status for email and phone</returns>
        [HttpGet("verification-status")]
        public async Task<IActionResult> GetVerificationStatus()
        {
            try
            {
                // Get current user ID from the token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var user = await _db.GetSingleDataAsync<IdentityDto>("Identity", userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new
                {
                    email = new
                    {
                        value = user.Email,
                        isVerified = user.IsEmailVerified,
                        verifiedAt = user.EmailVerifiedAt
                    },
                    phone = new
                    {
                        value = user.PhoneNumber,
                        isVerified = user.IsPhoneVerified,
                        verifiedAt = user.PhoneVerifiedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to get verification status", error = ex.Message });
            }
        }
    }
}
