using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Craftmatrix.org.Model;
using Craftmatrix.Codex.org.Service;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Craftmatrix.org.Data;
using System.IdentityModel.Tokens.Jwt;

namespace SIGLATAPI.Controllers.Reports
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IPostgreService _db;
        private readonly AppDBContext _context;

        public ReportController(IPostgreService db, AppDBContext context)
        {
            _db = db;
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetReports()
        {
            try
            {
                var reports = await _context.Reports
                    .Join(_context.Identity,
                          report => report.WhoReportedId,
                          identity => identity.Id,
                          (report, identity) => new ReportDto
                          {
                              Id = report.Id,
                              IncidentType = report.IncidentType,
                              Description = report.Description,
                              WhoReportedId = report.WhoReportedId,
                              Timestamp = report.Timestamp,
                              InvolvedAgencies = report.InvolvedAgencies,
                              Notes = report.Notes,
                              CreatedAt = report.CreatedAt,
                              UpdatedAt = report.UpdatedAt,
                              ReporterName = $"{identity.FirstName} {identity.LastName}",
                              ReporterEmail = identity.Email
                          })
                    .OrderByDescending(r => r.Timestamp)
                    .ToListAsync();

                return Ok(reports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve reports", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetReport(Guid id)
        {
            try
            {
                var report = await _context.Reports
                    .Join(_context.Identity,
                          report => report.WhoReportedId,
                          identity => identity.Id,
                          (report, identity) => new ReportDto
                          {
                              Id = report.Id,
                              IncidentType = report.IncidentType,
                              Description = report.Description,
                              WhoReportedId = report.WhoReportedId,
                              Timestamp = report.Timestamp,
                              InvolvedAgencies = report.InvolvedAgencies,
                              Notes = report.Notes,
                              CreatedAt = report.CreatedAt,
                              UpdatedAt = report.UpdatedAt,
                              ReporterName = $"{identity.FirstName} {identity.LastName}",
                              ReporterEmail = identity.Email
                          })
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (report == null)
                {
                    return NotFound(new { message = "Report not found" });
                }

                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve report", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateReport([FromBody] ReportDto reportDto)
        {
            try
            {
                // Debug: Log all available claims
                Console.WriteLine("=== JWT Claims Debug ===");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
                }
                Console.WriteLine("========================");
                
                // Get the current user's ID from the JWT token
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                Console.WriteLine($"Jti claim value: {userIdClaim}");
                
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    Console.WriteLine($"Failed to parse user ID. Claim value: '{userIdClaim}'");
                    return BadRequest(new { message = "Invalid user ID" });
                }

                // Set the reporter ID to the current user
                reportDto.WhoReportedId = userId;
                reportDto.Id = Guid.NewGuid();
                reportDto.CreatedAt = DateTime.UtcNow;
                reportDto.UpdatedAt = DateTime.UtcNow;

                // Clear navigation properties before saving
                reportDto.ReporterName = null;
                reportDto.ReporterEmail = null;

                await _db.PostDataAsync<ReportDto>("Reports", reportDto, reportDto.Id);

                // Return the created report with reporter info
                var createdReport = await GetReport(reportDto.Id);
                return CreatedAtAction(nameof(GetReport), new { id = reportDto.Id }, ((OkObjectResult)createdReport).Value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create report", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateReport(Guid id, [FromBody] ReportDto reportDto)
        {
            try
            {
                if (id != reportDto.Id)
                {
                    return BadRequest(new { message = "ID mismatch" });
                }

                // Ensure the report exists
                var existingReport = await _context.Reports.FindAsync(id);
                if (existingReport == null)
                {
                    return NotFound(new { message = "Report not found" });
                }

                // Preserve original reporter and creation date
                reportDto.WhoReportedId = existingReport.WhoReportedId;
                reportDto.CreatedAt = existingReport.CreatedAt;
                reportDto.UpdatedAt = DateTime.UtcNow;

                // Clear navigation properties before saving
                reportDto.ReporterName = null;
                reportDto.ReporterEmail = null;

                await _db.PostDataAsync<ReportDto>("Reports", reportDto, reportDto.Id);

                // Return the updated report with reporter info
                var updatedReport = await GetReport(reportDto.Id);
                return Ok(((OkObjectResult)updatedReport).Value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update report", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReport(Guid id)
        {
            try
            {
                await _db.DeleteDataAsync("Reports", id);
                return Ok(new { message = "Report deleted successfully", id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete report", error = ex.Message });
            }
        }

        [HttpGet("analytics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAnalytics()
        {
            try
            {
                var reports = await _context.Reports.ToListAsync();

                var analytics = new
                {
                    totalReports = reports.Count,
                    byIncidentType = reports.GroupBy(r => r.IncidentType)
                                          .Select(g => new { type = g.Key, count = g.Count() })
                                          .ToList(),
                    byAgencies = reports.Where(r => !string.IsNullOrEmpty(r.InvolvedAgencies))
                                       .SelectMany(r => r.InvolvedAgencies.Split(',', StringSplitOptions.RemoveEmptyEntries))
                                       .GroupBy(agency => agency.Trim())
                                       .Select(g => new { agency = g.Key, count = g.Count() })
                                       .ToList(),
                    recentReports = reports.OrderByDescending(r => r.Timestamp)
                                          .Take(5)
                                          .Select(r => new
                                          {
                                              id = r.Id,
                                              incidentType = r.IncidentType,
                                              timestamp = r.Timestamp
                                          })
                                          .ToList()
                };

                return Ok(analytics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to get analytics", error = ex.Message });
            }
        }
    }
}