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
    public class IAMController : ControllerBase
    {
        private readonly IPostgreService _db;
        private readonly IHttpClientFactory _httpClientFactory;

        public IAMController(IPostgreService db, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;

        }
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

        [HttpPost]
        public async Task<IActionResult> UpdateIAM([FromBody] IdentityDto identityDto)
        {
            // var whos = await _db.GetDataAsync<IdentityDto>("Identity");
            var whoami = await _db.PostDataAsync<IdentityDto>("Identity", identityDto, identityDto.Id);
            return Ok(whoami);
            // return Ok(tokenData.ToString());
        }

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

                await _db.PostDataAsync<VerificationDto>("Verifications", verification, verification.Id);
                return Ok("Verify?");
            }
        }


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
