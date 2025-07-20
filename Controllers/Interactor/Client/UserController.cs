using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SIGLAT.API.Model;

namespace SIGLAT.API.Controllers.Client
{
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
        [HttpPost("coordinates")]
        public async Task<IActionResult> Get([FromBody] UserXYZDto user)
        {
            await _db.PostDataAsync<UserXYZDto>("UserXYZ", user, user.Id);
            return Ok("Success");
        }
    }
}
