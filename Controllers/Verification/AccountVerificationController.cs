using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Craftmatrix.org.API.Data;
using Craftmatrix.org.API.Models;

namespace Craftmatrix.org.API.Controllers.Verification
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountVerificationController : ControllerBase
    {
        private readonly AppDBContext _context;

        public AccountVerificationController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet("types")]
        [AllowAnonymous]
        public async Task<ActionResult<List<VerificationTypeDto>>> GetVerificationTypes()
        {
            try
            {
                var types = await _context.VerificationTypes
                    .Where(vt => vt.IsActive)
                    .OrderBy(vt => vt.Name)
                    .ToListAsync();

                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving verification types.", error = ex.Message });
            }
        }

        [HttpGet("status")]
        public async Task<ActionResult<AccountVerificationResponseDto?>> GetVerificationStatus()
        {
            try
            {
                var userIdString = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in token." });
                }

                var query = from av in _context.AccountVerifications
                           join vt in _context.VerificationTypes on av.VerificationTypeId equals vt.Id
                           where av.UserId == userId
                           orderby av.CreatedAt descending
                           select new AccountVerificationResponseDto
                           {
                               Id = av.Id,
                               UserId = av.UserId,
                               VerificationTypeId = av.VerificationTypeId,
                               DocumentNumber = av.DocumentNumber,
                               DocumentName = av.DocumentName,
                               Status = av.Status,
                               AdminNotes = av.AdminNotes,
                               ReviewedByUserId = av.ReviewedByUserId,
                               ReviewedAt = av.ReviewedAt,
                               CreatedAt = av.CreatedAt,
                               UpdatedAt = av.UpdatedAt,
                               VerificationTypeName = vt.Name,
                               VerificationTypeDescription = vt.Description
                           };

                var verification = await query.FirstOrDefaultAsync();

                if (verification == null)
                {
                    return Ok(null);
                }

                return Ok(verification);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving verification status.", error = ex.Message });
            }
        }

        [HttpPost("submit")]
        public async Task<ActionResult<AccountVerificationResponseDto>> SubmitVerification([FromForm] SubmitVerificationDto request)
        {
            try
            {
                var userIdString = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in token." });
                }

                // Check if verification type exists
                var verificationType = await _context.VerificationTypes
                    .FirstOrDefaultAsync(vt => vt.Id == request.VerificationTypeId && vt.IsActive);

                if (verificationType == null)
                {
                    return BadRequest(new { message = "Invalid verification type." });
                }

                // Check if user already has a pending or approved verification
                var existingVerification = await _context.AccountVerifications
                    .Where(av => av.UserId == userId)
                    .OrderByDescending(av => av.CreatedAt)
                    .FirstOrDefaultAsync();

                if (existingVerification != null && 
                    (existingVerification.Status == VerificationStatus.Pending || 
                     existingVerification.Status == VerificationStatus.UnderReview ||
                     existingVerification.Status == VerificationStatus.Approved))
                {
                    return BadRequest(new { message = "You already have a verification request that is pending, under review, or approved." });
                }

                // Process uploaded image
                byte[]? imageData = null;
                string? mimeType = null;

                if (request.DocumentImage != null)
                {
                    // Validate file type
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(request.DocumentImage.ContentType?.ToLower()))
                    {
                        return BadRequest(new { message = "Invalid image format. Only JPEG, PNG, and GIF are allowed." });
                    }

                    // Validate file size (5MB limit)
                    if (request.DocumentImage.Length > 5 * 1024 * 1024)
                    {
                        return BadRequest(new { message = "Image size must be less than 5MB." });
                    }

                    using var memoryStream = new MemoryStream();
                    await request.DocumentImage.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                    mimeType = request.DocumentImage.ContentType;
                }

                var verification = new AccountVerificationDto
                {
                    UserId = userId,
                    VerificationTypeId = request.VerificationTypeId,
                    DocumentNumber = request.DocumentNumber ?? string.Empty,
                    DocumentName = request.DocumentName ?? string.Empty,
                    DocumentImage = imageData,
                    ImageMimeType = mimeType,
                    Status = VerificationStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.AccountVerifications.Add(verification);
                await _context.SaveChangesAsync();

                var response = new AccountVerificationResponseDto
                {
                    Id = verification.Id,
                    UserId = verification.UserId,
                    VerificationTypeId = verification.VerificationTypeId,
                    DocumentNumber = verification.DocumentNumber,
                    DocumentName = verification.DocumentName,
                    Status = verification.Status,
                    AdminNotes = verification.AdminNotes,
                    ReviewedByUserId = verification.ReviewedByUserId,
                    ReviewedAt = verification.ReviewedAt,
                    CreatedAt = verification.CreatedAt,
                    UpdatedAt = verification.UpdatedAt,
                    VerificationTypeName = verificationType.Name,
                    VerificationTypeDescription = verificationType.Description
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while submitting verification.", error = ex.Message });
            }
        }

        [HttpGet("image/{verificationId}")]
        public async Task<IActionResult> GetVerificationImage(int verificationId)
        {
            try
            {
                var userIdString = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in token." });
                }

                var verification = await _context.AccountVerifications
                    .Where(av => av.Id == verificationId && av.UserId == userId)
                    .FirstOrDefaultAsync();

                if (verification == null)
                {
                    return NotFound(new { message = "Verification not found." });
                }

                if (verification.DocumentImage == null)
                {
                    return NotFound(new { message = "No image found for this verification." });
                }

                return File(verification.DocumentImage, verification.ImageMimeType ?? "application/octet-stream");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving image.", error = ex.Message });
            }
        }
    }

    public class SubmitVerificationDto
    {
        public int VerificationTypeId { get; set; }
        public string? DocumentNumber { get; set; }
        public string? DocumentName { get; set; }
        public IFormFile? DocumentImage { get; set; }
    }
}