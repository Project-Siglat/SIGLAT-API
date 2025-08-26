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
    /// <summary>
    /// Controller for managing emergency incident reports in the SIGLAT system
    /// </summary>
    /// <remarks>
    /// This controller handles the creation, retrieval, updating, and deletion of incident reports,
    /// as well as providing analytics for administrative oversight. All endpoints require Admin role authorization.
    /// </remarks>
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

        /// <summary>
        /// Retrieves all incident reports with reporter information
        /// </summary>
        /// <returns>A list of all incident reports ordered by timestamp (most recent first)</returns>
        /// <response code="200">Reports retrieved successfully</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="500">Internal server error occurred while retrieving reports</response>
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

        /// <summary>
        /// Retrieves a specific incident report by its unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the report to retrieve</param>
        /// <returns>The specified incident report with reporter information</returns>
        /// <response code="200">Report retrieved successfully</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="404">Report not found</response>
        /// <response code="500">Internal server error occurred while retrieving the report</response>
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

        /// <summary>
        /// Creates a new incident report
        /// </summary>
        /// <param name="reportDto">The report data to create</param>
        /// <returns>The created report with populated system fields</returns>
        /// <remarks>
        /// The reporter ID is automatically set from the authenticated user's JWT token.
        /// System-generated fields like Id, CreatedAt, and UpdatedAt are automatically populated.
        /// </remarks>
        /// <response code="201">Report created successfully</response>
        /// <response code="400">Invalid request data or user ID</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="500">Internal server error occurred while creating the report</response>
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

        /// <summary>
        /// Updates an existing incident report
        /// </summary>
        /// <param name="id">The unique identifier of the report to update</param>
        /// <param name="reportDto">The updated report data</param>
        /// <returns>The updated report with reporter information</returns>
        /// <remarks>
        /// The original reporter ID and creation date are preserved during updates.
        /// Only the UpdatedAt timestamp is modified to reflect the current time.
        /// </remarks>
        /// <response code="200">Report updated successfully</response>
        /// <response code="400">ID mismatch between route parameter and request body</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="404">Report not found</response>
        /// <response code="500">Internal server error occurred while updating the report</response>
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

        /// <summary>
        /// Deletes an incident report
        /// </summary>
        /// <param name="id">The unique identifier of the report to delete</param>
        /// <returns>Confirmation message with the deleted report ID</returns>
        /// <response code="200">Report deleted successfully</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="404">Report not found (implicitly handled by delete operation)</response>
        /// <response code="500">Internal server error occurred while deleting the report</response>
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

        /// <summary>
        /// Retrieves analytical data about incident reports
        /// </summary>
        /// <returns>Analytics including total counts, incident type breakdowns, agency involvement, and recent reports</returns>
        /// <remarks>
        /// Provides comprehensive analytics including:
        /// - Total report count
        /// - Report distribution by incident type
        /// - Report distribution by involved agencies
        /// - Five most recent reports
        /// </remarks>
        /// <response code="200">Analytics retrieved successfully</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="500">Internal server error occurred while generating analytics</response>
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