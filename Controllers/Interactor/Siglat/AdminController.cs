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
    // [Authorize]
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

        [HttpGet("admin")]
        public async Task<IActionResult> Admin()
        {
            var admins = await _db.GetDataAsync<IdentityDto>("Identity");
            var admin = admins.Where(x => x.Role == "Admin");
            if (admin == null || !admin.Any())
            {
                var adminUser = new IdentityDto
                {
                    Id = Guid.NewGuid(),
                    FirstName = "admin",
                    MiddleName = "",
                    LastName = "",
                    Address = "",
                    Gender = "",
                    PhoneNumber = "",
                    Role = "Admin",
                    DateOfBirth = DateTime.MinValue,
                    Email = "admin@gmail.com",
                    HashPass = PasswordService.HashPassword("1234"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var data = await _db.PostDataAsync<IdentityDto>("Identity", adminUser, adminUser.Id);
                return Ok(data);
            }

            return Ok(admin);
        }

        [HttpGet("userlist")]
        [AllowAnonymous]
        public async Task<IActionResult> UserLit()
        {
            var data = await _db.GetDataAsync<IdentityDto>("Identity");
            return Ok(data);
        }

        [HttpDelete("user")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteUser(Guid Id)
        {
            await _db.DeleteDataAsync("Identity", Id);
            return Ok(Id);
        }

        [HttpPost("verification-action")]
        [AllowAnonymous]
        public async Task<IActionResult> VerificationAction([FromBody] VerificationDto Verification)
        {
            Verification.UpdatedAt = DateTime.UtcNow;
            await _db.PostDataAsync<VerificationDto>("Verifications", Verification, Verification.Id);
            return Ok(Verification);
        }


        [HttpPost("update-user")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateUser([FromBody] IdentityDto User)
        {
            User.UpdatedAt = DateTime.UtcNow;
            var data = await _db.GetSingleDataAsync<IdentityDto>("Identity", User.Id);
            User.HashPass = data.HashPass;

            await _db.PostDataAsync<IdentityDto>("Identity", User, User.Id);
            return Ok(User);
        }

        [HttpGet("verify")]
        [AllowAnonymous]
        public async Task<IActionResult> Verify()
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
            }).ToList();

            return Ok(verificationDetails);
        }

        [HttpGet("contact")]
        public async Task<IActionResult> Contacts()
        {
            var data = await _db.GetDataAsync<ContactDto>("Contact");
            data = data.OrderBy(x => x.ContactType).ToList();
            return Ok(data);
        }

        [HttpDelete("contact")]
        public async Task<IActionResult> DeleteContact(Guid Id)
        {
            await _db.DeleteDataAsync("Contact", Id);
            return Ok(Id);
        }

        [HttpPost("contact")]
        public async Task<IActionResult> Contact([FromBody] ContactDto Contact)
        {
            Contact.CreatedAt = DateTime.UtcNow;
            Contact.UpdatedAt = DateTime.UtcNow;
            await _db.PostDataAsync<ContactDto>("Contact", Contact, Contact.Id);
            return Ok(Contact);
        }
    }
}
