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
using System.Security.Cryptography;

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
        private readonly IEmailService _emailService;

        public AuthController(IPostgreService db, IHttpClientFactory httpClientFactory, IEmailService emailService)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _emailService = emailService;

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
                    // Get the role name instead of role ID
                    var roles = await _db.GetDataByColumnAsync<RoleDto>("Roles", "Id", data.RoleId);
                    var roleName = roles.FirstOrDefault()?.Name ?? "User"; // Default to "User" if role not found
                    
                    var accessToken = GenerateToken(data.Email, data.Id.ToString(), roleName, data.RoleId);
                    var refreshToken = await GenerateRefreshToken(data.Id, ipAddress, userAgent);

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

                    var tokenResponse = new TokenResponseDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken.Token,
                        AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
                        RefreshTokenExpiresAt = refreshToken.ExpiresAt,
                        RoleId = data.RoleId,
                        Role = roleName, // Add role name to response
                        UserId = data.Id
                    };

                    return Ok(tokenResponse);
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

        private string GenerateToken(string email, string userId, string roleName, int? roleId = null)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrEmpty(roleName)) throw new ArgumentNullException(nameof(roleName));

            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new InvalidOperationException("JWT_SECRET environment variable is not set.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, userId),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("role", roleName.ToLower()) // Add lowercase role for easier comparison
            };

            // Add roleId claim if provided (for backward compatibility)
            if (roleId.HasValue)
            {
                claims.Add(new Claim("roleId", roleId.Value.ToString()));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15), // Reduced to 15 minutes
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
        /// Get current user's login history for dashboard
        /// </summary>
        /// <returns>Current user's login history</returns>
        /// <response code="200">Login history retrieved successfully</response>
        /// <response code="401">Invalid or missing authentication token</response>
        [HttpGet("my-login-history")]
        public async Task<IActionResult> GetMyLoginHistory()
        {
            try
            {
                var jtiClaim = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(jtiClaim) || !Guid.TryParse(jtiClaim, out Guid userId))
                {
                    return Unauthorized("Invalid token");
                }

                var allLogs = await _db.GetDataAsync<UserLoginTrackingDto>("UserLoginTracking");
                var userLogs = allLogs
                    .Where(l => l.UserId == userId)
                    .OrderByDescending(l => l.LoginTimestamp)
                    .Take(50) // Last 50 login attempts
                    .Select(l => new
                    {
                        Id = l.Id.ToString(),
                        UserId = l.UserId.ToString(),
                        l.IpAddress,
                        l.UserAgent,
                        l.LoginTimestamp,
                        l.LogoutTimestamp,
                        l.LoginStatus,
                        l.FailureReason,
                        l.AttemptedEmail,
                        l.IsActive
                    });

                return Ok(userLogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving login history: {ex.Message}");
            }
        }

        /// <summary>
        /// Upgrade user to admin role (temporary endpoint)
        /// </summary>
        [HttpPost("make-admin/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> MakeUserAdmin(string email)
        {
            try
            {
                var users = await _db.GetDataByColumnAsync<IdentityDto>("Identity", "Email", email);
                var user = users.FirstOrDefault();
                
                if (user == null)
                {
                    return NotFound("User not found");
                }
                
                // Update user's role to admin (roleId = 1)
                user.RoleId = 1;
                user.UpdatedAt = DateTime.UtcNow;
                
                await _db.PostDataAsync("Identity", user, user.Id);
                
                return Ok(new { 
                    message = $"User {email} upgraded to admin successfully",
                    userId = user.Id,
                    roleId = user.RoleId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error upgrading user: {ex.Message}");
            }
        }

        /// <summary>
        /// Get login tracking logs (Admin only)
        /// </summary>
        /// <returns>Login tracking data</returns>
        [HttpGet("login-logs")]
        [Authorize(Roles = "Admin")] // Admin role only
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
        /// Logout user and mark session as inactive
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var jtiClaim = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (!string.IsNullOrEmpty(jtiClaim) && Guid.TryParse(jtiClaim, out Guid userId))
                {
                    // Update the most recent active login session to mark logout time
                    var allLogs = await _db.GetDataAsync<UserLoginTrackingDto>("UserLoginTracking");
                    var activeSession = allLogs
                        .Where(l => l.UserId == userId && l.IsActive && l.LoginStatus == "Success")
                        .OrderByDescending(l => l.LoginTimestamp)
                        .FirstOrDefault();

                    if (activeSession != null)
                    {
                        activeSession.LogoutTimestamp = DateTime.UtcNow;
                        activeSession.IsActive = false;
                        await _db.PostDataAsync<UserLoginTrackingDto>("UserLoginTracking", activeSession, activeSession.Id);
                    }
                }

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error during logout: {ex.Message}");
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

                // Send verification code via email if it's email verification
                if (request.VerificationType.ToLower() == "email")
                {
                    var emailSent = await _emailService.SendOtpEmailAsync(request.ContactValue, verificationCode, $"{user.FirstName} {user.LastName}");
                    if (!emailSent)
                    {
                        Console.WriteLine($"Failed to send verification email to {request.ContactValue}, but code was generated: {verificationCode}");
                    }
                }
                // TODO: Implement SMS sending for phone verification

                return Ok(new
                {
                    message = $"Verification code sent to your {request.VerificationType}",
                    expiresAt = token.ExpiresAt
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

        /// <summary>
        /// Generate a refresh token for the user
        /// </summary>
        private async Task<RefreshTokenDto> GenerateRefreshToken(Guid userId, string ipAddress, string userAgent)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var randomBytes = new byte[64];
                rng.GetBytes(randomBytes);
                var token = Convert.ToBase64String(randomBytes);

                var refreshToken = new RefreshTokenDto
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 days for refresh token
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _db.PostDataAsync<RefreshTokenDto>("RefreshTokens", refreshToken, refreshToken.Id);
                return refreshToken;
            }
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <param name="request">Refresh token request</param>
        /// <returns>New access and refresh tokens</returns>
        /// <response code="200">Tokens refreshed successfully</response>
        /// <response code="400">Invalid or expired refresh token</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RefreshToken))
                {
                    return BadRequest(new { message = "Refresh token is required" });
                }

                // Find the refresh token in database
                var refreshTokens = await _db.GetDataByColumnAsync<RefreshTokenDto>("RefreshTokens", "Token", request.RefreshToken);
                var refreshToken = refreshTokens.FirstOrDefault();

                if (refreshToken == null || !refreshToken.IsActive)
                {
                    return BadRequest(new { message = "Invalid or expired refresh token" });
                }

                // Get user data
                var user = await _db.GetSingleDataAsync<IdentityDto>("Identity", refreshToken.UserId);
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }

                // Get role name
                var roles = await _db.GetDataByColumnAsync<RoleDto>("Roles", "Id", user.RoleId);
                var roleName = roles.FirstOrDefault()?.Name ?? "User";

                // Generate new tokens
                var newAccessToken = GenerateToken(user.Email, user.Id.ToString(), roleName, user.RoleId);
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                var newRefreshToken = await GenerateRefreshToken(user.Id, ipAddress, userAgent);

                // Revoke old refresh token
                refreshToken.IsRevoked = true;
                refreshToken.RevokedAt = DateTime.UtcNow;
                refreshToken.UpdatedAt = DateTime.UtcNow;
                await _db.PostDataAsync<RefreshTokenDto>("RefreshTokens", refreshToken, refreshToken.Id);

                var tokenResponse = new TokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken.Token,
                    AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    RefreshTokenExpiresAt = newRefreshToken.ExpiresAt,
                    RoleId = user.RoleId,
                    Role = roleName, // Add role name to response
                    UserId = user.Id
                };

                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to refresh token", error = ex.Message });
            }
        }

        /// <summary>
        /// Revoke refresh token
        /// </summary>
        /// <param name="request">Refresh token to revoke</param>
        /// <returns>Success message</returns>
        [HttpPost("revoke")]
        [AllowAnonymous]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RefreshToken))
                {
                    return BadRequest(new { message = "Refresh token is required" });
                }

                var refreshTokens = await _db.GetDataByColumnAsync<RefreshTokenDto>("RefreshTokens", "Token", request.RefreshToken);
                var refreshToken = refreshTokens.FirstOrDefault();

                if (refreshToken != null && refreshToken.IsActive)
                {
                    refreshToken.IsRevoked = true;
                    refreshToken.RevokedAt = DateTime.UtcNow;
                    refreshToken.UpdatedAt = DateTime.UtcNow;
                    await _db.PostDataAsync<RefreshTokenDto>("RefreshTokens", refreshToken, refreshToken.Id);
                }

                return Ok(new { message = "Token revoked successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to revoke token", error = ex.Message });
            }
        }

        /// <summary>
        /// Clean up expired refresh tokens (should be called periodically)
        /// </summary>
        [HttpDelete("cleanup-tokens")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CleanupExpiredTokens()
        {
            try
            {
                var allTokens = await _db.GetDataAsync<RefreshTokenDto>("RefreshTokens");
                var expiredTokens = allTokens.Where(t => t.IsExpired || t.IsRevoked).ToList();

                int deletedCount = 0;
                foreach (var token in expiredTokens)
                {
                    await _db.DeleteDataAsync("RefreshTokens", token.Id);
                    deletedCount++;
                }

                return Ok(new { message = $"Cleaned up {deletedCount} expired/revoked tokens" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to cleanup tokens", error = ex.Message });
            }
        }

        /// <summary>
        /// Debug endpoint to inspect JWT token claims (temporary)
        /// </summary>
        [HttpGet("debug-token")]
        public async Task<IActionResult> DebugToken()
        {
            try
            {
                var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                var email = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                
                return Ok(new
                {
                    IsAuthenticated = User.Identity?.IsAuthenticated,
                    AuthenticationType = User.Identity?.AuthenticationType,
                    Role = userRole,
                    UserId = userId,
                    Email = email,
                    AllClaims = claims,
                    IsInAdminRole = User.IsInRole("Admin"),
                    Identity = User.Identity?.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to debug token", error = ex.Message });
            }
        }

        /// <summary>
        /// Send OTP for admin registration (no authentication required)
        /// </summary>
        /// <param name="request">Email for admin registration</param>
        /// <returns>Success message with expiration time</returns>
        [HttpPost("send-admin-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> SendAdminOtp([FromBody] SendAdminOtpDto request)
        {
            try
            {
                // Check if admin already exists
                var adminCheck = await _db.GetDataByColumnAsync<IdentityDto>("Identity", "RoleId", 1);
                if (adminCheck.Any())
                {
                    return BadRequest(new { message = "Admin account already exists in the system" });
                }

                // Check for recent unused tokens for this email
                var recentTokens = await _db.GetDataAsync<AdminVerificationTokenDto>("AdminVerificationTokens");
                var existingToken = recentTokens.FirstOrDefault(t =>
                    t.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) &&
                    !t.IsUsed &&
                    t.ExpiresAt > DateTime.UtcNow &&
                    t.CreatedAt > DateTime.UtcNow.AddMinutes(-2)); // Don't allow new codes within 2 minutes

                if (existingToken != null)
                {
                    var remainingTime = existingToken.ExpiresAt.Subtract(DateTime.UtcNow);
                    return BadRequest(new
                    {
                        message = "An OTP was recently sent to this email. Please wait before requesting a new one.",
                        retryAfter = remainingTime.TotalSeconds
                    });
                }

                // Generate 6-digit OTP
                var random = new Random();
                var otpCode = random.Next(100000, 999999).ToString();

                // Create admin verification token
                var token = new AdminVerificationTokenDto
                {
                    Email = request.Email.ToLower(),
                    VerificationCode = otpCode,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10), // 10 minutes expiration
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
                };

                await _db.PostDataAsync<AdminVerificationTokenDto>("AdminVerificationTokens", token, token.Id);

                // Send OTP via email using Resend
                var emailSent = await _emailService.SendOtpEmailAsync(request.Email, otpCode, "Admin User");
                
                if (!emailSent)
                {
                    // Log the failure but still return success to avoid revealing system details
                    Console.WriteLine($"Failed to send email to {request.Email}, but OTP was generated: {otpCode}");
                }

                return Ok(new
                {
                    message = $"OTP sent to {request.Email}",
                    expiresAt = token.ExpiresAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to send admin OTP", error = ex.Message });
            }
        }

        /// <summary>
        /// Verify OTP for admin registration
        /// </summary>
        /// <param name="request">Email and OTP code verification</param>
        /// <returns>Success message if OTP is valid</returns>
        [HttpPost("verify-admin-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyAdminOtp([FromBody] VerifyAdminOtpDto request)
        {
            try
            {
                // Check if admin already exists
                var adminCheck = await _db.GetDataByColumnAsync<IdentityDto>("Identity", "RoleId", 1);
                if (adminCheck.Any())
                {
                    return BadRequest(new { message = "Admin account already exists in the system" });
                }

                // Find the verification token
                var tokens = await _db.GetDataAsync<AdminVerificationTokenDto>("AdminVerificationTokens");
                var token = tokens.FirstOrDefault(t =>
                    t.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) &&
                    !t.IsUsed &&
                    t.ExpiresAt > DateTime.UtcNow);

                if (token == null)
                {
                    return BadRequest(new { message = "Invalid or expired OTP" });
                }

                // Check attempt count
                if (token.AttemptCount >= 3)
                {
                    return BadRequest(new { message = "Too many OTP attempts. Please request a new OTP." });
                }

                // Increment attempt count
                token.AttemptCount++;
                await _db.PostDataAsync<AdminVerificationTokenDto>("AdminVerificationTokens", token, token.Id);

                // Verify the code
                if (token.VerificationCode != request.VerificationCode)
                {
                    return BadRequest(new
                    {
                        message = "Invalid OTP",
                        attemptsRemaining = 3 - token.AttemptCount
                    });
                }

                // Mark token as used
                token.IsUsed = true;
                token.VerifiedAt = DateTime.UtcNow;
                await _db.PostDataAsync<AdminVerificationTokenDto>("AdminVerificationTokens", token, token.Id);

                return Ok(new
                {
                    message = "OTP verified successfully. You can now create the admin account.",
                    verified = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to verify admin OTP", error = ex.Message });
            }
        }

        /// <summary>
        /// Check for existing email verification session
        /// </summary>
        /// <returns>Existing session details if available</returns>
        [HttpGet("email-verification-status")]
        public async Task<IActionResult> GetEmailVerificationStatus()
        {
            try
            {
                // Get current user ID from the token
                var jtiClaim = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(jtiClaim) || !Guid.TryParse(jtiClaim, out Guid userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                // Get user
                var user = await _db.GetSingleDataAsync<IdentityDto>("Identity", userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Check if already verified
                if (user.IsEmailVerified)
                {
                    return Ok(new { 
                        isVerified = true, 
                        message = "Email is already verified" 
                    });
                }

                // Check for existing unused token
                var tokens = await _db.GetDataAsync<ContactVerificationTokenDto>("ContactVerificationTokens");
                var existingToken = tokens.FirstOrDefault(t =>
                    t.UserId == userId &&
                    t.VerificationType == "email" &&
                    t.ContactValue == user.Email &&
                    !t.IsUsed &&
                    t.ExpiresAt > DateTime.UtcNow);

                if (existingToken != null)
                {
                    var remainingTime = existingToken.ExpiresAt.Subtract(DateTime.UtcNow);
                    return Ok(new
                    {
                        isVerified = false,
                        hasActiveSession = true,
                        expiresAt = existingToken.ExpiresAt,
                        remainingTimeSeconds = remainingTime.TotalSeconds,
                        canResend = existingToken.CreatedAt <= DateTime.UtcNow.AddMinutes(-2) // Can resend after 2 minutes
                    });
                }

                return Ok(new
                {
                    isVerified = false,
                    hasActiveSession = false,
                    canSendNew = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to check verification status", error = ex.Message });
            }
        }

        /// <summary>
        /// Send email verification OTP (simplified endpoint for frontend compatibility)
        /// </summary>
        /// <param name="request">Email to send OTP to</param>
        /// <returns>Success message</returns>
        [HttpPost("send-email-otp")]
        public async Task<IActionResult> SendEmailOtp([FromBody] SendEmailOtpDto request)
        {
            try
            {
                // Get current user ID from the token
                var jtiClaim = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(jtiClaim) || !Guid.TryParse(jtiClaim, out Guid userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                // Get user to verify the email matches their profile
                var user = await _db.GetSingleDataAsync<IdentityDto>("Identity", userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Verify the email matches user's profile
                if (!user.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = "Email does not match your profile" });
                }

                // Check if already verified
                if (user.IsEmailVerified)
                {
                    return BadRequest(new { message = "Email is already verified" });
                }

                // Check for recent unused tokens
                var recentTokens = await _db.GetDataAsync<ContactVerificationTokenDto>("ContactVerificationTokens");
                var existingToken = recentTokens.FirstOrDefault(t =>
                    t.UserId == userId &&
                    t.VerificationType == "email" &&
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
                    VerificationType = "email",
                    ContactValue = request.Email,
                    VerificationCode = verificationCode,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10), // 10 minutes expiration
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
                };

                await _db.PostDataAsync<ContactVerificationTokenDto>("ContactVerificationTokens", token, token.Id);

                // Send verification code via email
                var emailSent = await _emailService.SendOtpEmailAsync(request.Email, verificationCode, $"{user.FirstName} {user.LastName}");
                if (!emailSent)
                {
                    Console.WriteLine($"Failed to send verification email to {request.Email}, but code was generated: {verificationCode}");
                }

                return Ok(new
                {
                    message = "Verification code sent to your email",
                    expiresAt = token.ExpiresAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to send verification code", error = ex.Message });
            }
        }

        /// <summary>
        /// Verify email with verification code (simplified endpoint for frontend compatibility)
        /// </summary>
        /// <param name="request">Verification code</param>
        /// <returns>Verification success message</returns>
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto request)
        {
            try
            {
                // Get current user ID from the token
                var jtiClaim = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(jtiClaim) || !Guid.TryParse(jtiClaim, out Guid userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                // Get user
                var user = await _db.GetSingleDataAsync<IdentityDto>("Identity", userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Find the verification token
                var tokens = await _db.GetDataAsync<ContactVerificationTokenDto>("ContactVerificationTokens");
                var token = tokens.FirstOrDefault(t =>
                    t.UserId == userId &&
                    t.VerificationType == "email" &&
                    t.ContactValue == user.Email &&
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
                user.IsEmailVerified = true;
                user.EmailVerifiedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                await _db.PostDataAsync<IdentityDto>("Identity", user, user.Id);

                return Ok(new
                {
                    message = "Email verified successfully",
                    isVerified = true,
                    verifiedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to verify email", error = ex.Message });
            }
        }

        /// <summary>
        /// Create admin account with verified OTP
        /// </summary>
        /// <param name="request">Email, OTP, and password for admin creation</param>
        /// <returns>Success message</returns>
        [HttpPost("create-admin-with-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAdminWithOtp([FromBody] CreateAdminWithOtpDto request)
        {
            try
            {
                // Check if admin already exists
                var adminCheck = await _db.GetDataByColumnAsync<IdentityDto>("Identity", "RoleId", 1);
                if (adminCheck.Any())
                {
                    return BadRequest(new { message = "Admin account already exists in the system" });
                }

                // Find a verified token for this email
                var tokens = await _db.GetDataAsync<AdminVerificationTokenDto>("AdminVerificationTokens");
                var verifiedToken = tokens.FirstOrDefault(t =>
                    t.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) &&
                    t.IsUsed &&
                    t.VerificationCode == request.VerificationCode &&
                    t.VerifiedAt > DateTime.UtcNow.AddMinutes(-15)); // Token must be verified within 15 minutes

                if (verifiedToken == null)
                {
                    return BadRequest(new { message = "Invalid or expired OTP verification. Please verify your OTP again." });
                }

                // Create admin user
                var adminUser = new IdentityDto
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Admin",
                    MiddleName = "",
                    LastName = "User",
                    Email = request.Email.ToLower(),
                    HashPass = PasswordService.HashPassword(request.Password),
                    RoleId = 1, // Admin role
                    IsEmailVerified = true, // Email is verified through OTP
                    EmailVerifiedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Address = "",
                    Gender = "",
                    PhoneNumber = "",
                    DateOfBirth = DateTime.UtcNow.AddYears(-25) // Default date
                };

                await _db.PostDataAsync<IdentityDto>("Identity", adminUser, adminUser.Id);

                // Clean up used tokens for this email
                var emailTokens = tokens.Where(t => t.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
                foreach (var tokenToDelete in emailTokens)
                {
                    await _db.DeleteDataAsync("AdminVerificationTokens", tokenToDelete.Id);
                }

                return Ok(new
                {
                    message = "Admin account created successfully",
                    adminId = adminUser.Id,
                    email = adminUser.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create admin account", error = ex.Message });
            }
        }

        /// <summary>
        /// TEMPORARY: Reset admin password (for development only)
        /// </summary>
        [HttpPost("reset-admin-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetAdminPassword([FromBody] ResetAdminPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.NewPassword))
                {
                    return BadRequest(new { message = "Email and new password are required" });
                }

                // Find admin user by email
                var adminUsers = await _db.GetDataByColumnAsync<IdentityDto>("Identity", "Email", request.Email);
                var admin = adminUsers.FirstOrDefault();

                if (admin == null)
                {
                    return NotFound(new { message = "Admin user not found" });
                }

                if (admin.RoleId != 1) // Ensure it's an admin
                {
                    return BadRequest(new { message = "User is not an admin" });
                }

                // Hash the new password
                var hashedPassword = PasswordService.HashPassword(request.NewPassword);

                // Update admin password
                admin.HashPass = hashedPassword;
                admin.UpdatedAt = DateTime.UtcNow;

                await _db.PostDataAsync<IdentityDto>("Identity", admin, admin.Id);

                return Ok(new { message = "Admin password reset successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to reset admin password", error = ex.Message });
            }
        }
    }

    public class ResetAdminPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
