using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SIGLAT.API.Model;
using Craftmatrix.org.Model;
namespace SIGLAT.API.Controllers.Ambulance
{
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

        [HttpPost("alert")]
        public async Task<IActionResult> Alert([FromBody] AlertDto alerto)
        {
            alerto.Id = Guid.NewGuid();
            alerto.RespondedAt = DateTime.UtcNow;
            await _db.PostDataAsync<AlertDto>("Alerts", alerto, alerto.Id);
            return Ok(new { message = "Alert posted successfully" });
        }

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
