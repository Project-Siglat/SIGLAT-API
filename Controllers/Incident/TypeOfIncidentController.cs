using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Craftmatrix.org.API.Models;
using Craftmatrix.org.API.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Craftmatrix.org.API.Controllers.Incident
{
    /// <summary>
    /// Controller for managing types of incidents in the emergency response system
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TypeOfIncidentController : ControllerBase
    {
        private readonly IPostgreService _db;

        public TypeOfIncidentController(IPostgreService db)
        {
            _db = db;
        }

        /// <summary>
        /// Get all incident types (accessible by all authenticated users)
        /// </summary>
        /// <returns>List of all active incident types</returns>
        /// <response code="200">Incident types retrieved successfully</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetIncidentTypes()
        {
            try
            {
                var data = await _db.GetDataAsync<TypeOfIncidentDto>("TypeOfIncidents");
                
                // Filter active incident types and order by name
                var activeIncidentTypes = data
                    .Where(incident => incident.IsActive)
                    .OrderBy(incident => incident.NameOfIncident)
                    .Select(incident => new 
                    {
                        incident.Id,
                        incident.NameOfIncident,
                        incident.Description,
                        incident.AddedDateTime,
                        incident.WhoAddedItID,
                        incident.IsActive,
                        incident.isBFPTrue,
                        incident.isPNPTrue,
                        incident.CreatedAt,
                        incident.UpdatedAt
                    });
                
                return Ok(activeIncidentTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve incident types", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all incident types including inactive ones (Admin only)
        /// </summary>
        /// <returns>List of all incident types</returns>
        /// <response code="200">All incident types retrieved successfully</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllIncidentTypes()
        {
            try
            {
                var data = await _db.GetDataAsync<TypeOfIncidentDto>("TypeOfIncidents");
                var identities = await _db.GetDataAsync<IdentityDto>("Identity");
                
                // Include who added each incident type
                var incidentTypesWithDetails = data
                    .OrderByDescending(incident => incident.CreatedAt)
                    .Select(incident => new 
                    {
                        incident.Id,
                        incident.NameOfIncident,
                        incident.Description,
                        incident.AddedDateTime,
                        incident.WhoAddedItID,
                        AddedBy = identities.FirstOrDefault(user => user.Id == incident.WhoAddedItID)?.FirstName + " " +
                                 identities.FirstOrDefault(user => user.Id == incident.WhoAddedItID)?.LastName,
                        incident.IsActive,
                        incident.isBFPTrue,
                        incident.isPNPTrue,
                        incident.CreatedAt,
                        incident.UpdatedAt
                    });
                
                return Ok(incidentTypesWithDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve all incident types", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific incident type by ID
        /// </summary>
        /// <param name="id">Incident type ID</param>
        /// <returns>Incident type details</returns>
        /// <response code="200">Incident type retrieved successfully</response>
        /// <response code="404">Incident type not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetIncidentType(Guid id)
        {
            try
            {
                var incidentType = await _db.GetSingleDataAsync<TypeOfIncidentDto>("TypeOfIncidents", id);
                if (incidentType == null)
                {
                    return NotFound(new { message = "Incident type not found" });
                }
                
                return Ok(incidentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve incident type", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new incident type (Admin only)
        /// </summary>
        /// <param name="incidentType">Incident type data</param>
        /// <returns>Created incident type</returns>
        /// <response code="201">Incident type created successfully</response>
        /// <response code="400">Invalid data provided</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateIncidentType([FromBody] TypeOfIncidentDto incidentType)
        {
            try
            {
                // Get current user ID from JWT token
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value ?? 
                                 User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                                 User.FindFirst("nameid")?.Value ?? 
                                 User.FindFirst("sub")?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return BadRequest(new { message = "Invalid user ID in token" });
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(incidentType.NameOfIncident))
                {
                    return BadRequest(new { message = "Incident name is required" });
                }

                // Check if incident type with same name already exists
                var existingIncidentTypes = await _db.GetDataAsync<TypeOfIncidentDto>("TypeOfIncidents");
                if (existingIncidentTypes.Any(x => x.NameOfIncident.ToLower() == incidentType.NameOfIncident.ToLower() && x.IsActive))
                {
                    return BadRequest(new { message = "An incident type with this name already exists" });
                }

                // Set system fields
                incidentType.Id = Guid.NewGuid();
                incidentType.WhoAddedItID = userId;
                incidentType.AddedDateTime = DateTime.UtcNow;
                incidentType.CreatedAt = DateTime.UtcNow;
                incidentType.UpdatedAt = DateTime.UtcNow;
                incidentType.IsActive = true;

                await _db.PostDataAsync<TypeOfIncidentDto>("TypeOfIncidents", incidentType, incidentType.Id);
                
                return CreatedAtAction(nameof(GetIncidentType), new { id = incidentType.Id }, incidentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create incident type", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing incident type (Admin only)
        /// </summary>
        /// <param name="id">Incident type ID</param>
        /// <param name="incidentType">Updated incident type data</param>
        /// <returns>Updated incident type</returns>
        /// <response code="200">Incident type updated successfully</response>
        /// <response code="400">Invalid data provided</response>
        /// <response code="404">Incident type not found</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateIncidentType(Guid id, [FromBody] TypeOfIncidentDto incidentType)
        {
            try
            {
                if (id != incidentType.Id)
                {
                    return BadRequest(new { message = "Incident type ID mismatch" });
                }

                var existingIncidentType = await _db.GetSingleDataAsync<TypeOfIncidentDto>("TypeOfIncidents", id);
                if (existingIncidentType == null)
                {
                    return NotFound(new { message = "Incident type not found" });
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(incidentType.NameOfIncident))
                {
                    return BadRequest(new { message = "Incident name is required" });
                }

                // Check if another incident type with same name already exists
                var allIncidentTypes = await _db.GetDataAsync<TypeOfIncidentDto>("TypeOfIncidents");
                if (allIncidentTypes.Any(x => x.Id != id && x.NameOfIncident.ToLower() == incidentType.NameOfIncident.ToLower() && x.IsActive))
                {
                    return BadRequest(new { message = "An incident type with this name already exists" });
                }

                // Preserve system fields
                incidentType.WhoAddedItID = existingIncidentType.WhoAddedItID;
                incidentType.AddedDateTime = existingIncidentType.AddedDateTime;
                incidentType.CreatedAt = existingIncidentType.CreatedAt;
                incidentType.UpdatedAt = DateTime.UtcNow;

                await _db.PostDataAsync<TypeOfIncidentDto>("TypeOfIncidents", incidentType, incidentType.Id);
                
                return Ok(incidentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update incident type", error = ex.Message });
            }
        }

        /// <summary>
        /// Soft delete an incident type (set IsActive to false) (Admin only)
        /// </summary>
        /// <param name="id">Incident type ID</param>
        /// <returns>Success message</returns>
        /// <response code="200">Incident type deactivated successfully</response>
        /// <response code="404">Incident type not found</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateIncidentType(Guid id)
        {
            try
            {
                var existingIncidentType = await _db.GetSingleDataAsync<TypeOfIncidentDto>("TypeOfIncidents", id);
                if (existingIncidentType == null)
                {
                    return NotFound(new { message = "Incident type not found" });
                }

                // Soft delete by setting IsActive to false
                existingIncidentType.IsActive = false;
                existingIncidentType.UpdatedAt = DateTime.UtcNow;

                await _db.PostDataAsync<TypeOfIncidentDto>("TypeOfIncidents", existingIncidentType, existingIncidentType.Id);
                
                return Ok(new { message = "Incident type deactivated successfully", id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to deactivate incident type", error = ex.Message });
            }
        }

        /// <summary>
        /// Reactivate a deactivated incident type (Admin only)
        /// </summary>
        /// <param name="id">Incident type ID</param>
        /// <returns>Success message</returns>
        /// <response code="200">Incident type reactivated successfully</response>
        /// <response code="404">Incident type not found</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch("{id}/reactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReactivateIncidentType(Guid id)
        {
            try
            {
                var existingIncidentType = await _db.GetSingleDataAsync<TypeOfIncidentDto>("TypeOfIncidents", id);
                if (existingIncidentType == null)
                {
                    return NotFound(new { message = "Incident type not found" });
                }

                // Reactivate by setting IsActive to true
                existingIncidentType.IsActive = true;
                existingIncidentType.UpdatedAt = DateTime.UtcNow;

                await _db.PostDataAsync<TypeOfIncidentDto>("TypeOfIncidents", existingIncidentType, existingIncidentType.Id);
                
                return Ok(new { message = "Incident type reactivated successfully", id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to reactivate incident type", error = ex.Message });
            }
        }

        /// <summary>
        /// Hard delete an incident type (permanently remove from database) (Admin only)
        /// Use with caution - this cannot be undone
        /// </summary>
        /// <param name="id">Incident type ID</param>
        /// <returns>Success message</returns>
        /// <response code="200">Incident type permanently deleted</response>
        /// <response code="404">Incident type not found</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}/permanent")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PermanentlyDeleteIncidentType(Guid id)
        {
            try
            {
                var existingIncidentType = await _db.GetSingleDataAsync<TypeOfIncidentDto>("TypeOfIncidents", id);
                if (existingIncidentType == null)
                {
                    return NotFound(new { message = "Incident type not found" });
                }

                await _db.DeleteDataAsync("TypeOfIncidents", id);
                
                return Ok(new { message = "Incident type permanently deleted", id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to permanently delete incident type", error = ex.Message });
            }
        }
    }
}