using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAFuneralSuppliesAPI.Attributes;
using SAFuneralSuppliesAPI.Data;
using SAFuneralSuppliesAPI.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace SAFuneralSuppliesAPI.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
[AdminOnly]
public class AuditLogsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuditLogsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all audit logs with optional filtering (Admin only)
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "List audit logs",
        Description = "Get all authentication audit logs with optional filtering by action, email, and date range (Admin only)",
        Tags = new[] { "Admin - Audit Logs" }
    )]
    [SwaggerResponse(200, "Audit logs retrieved successfully", typeof(AuditLogsListResponse))]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(500, "Server error")]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery, SwaggerParameter("Filter by action (login, logout, login_failed)")] string? action,
        [FromQuery, SwaggerParameter("Filter by email")] string? email,
        [FromQuery, SwaggerParameter("Filter by role (customer, admin)")] string? role,
        [FromQuery, SwaggerParameter("Start date filter (ISO 8601)")] DateTime? from,
        [FromQuery, SwaggerParameter("End date filter (ISO 8601)")] DateTime? to,
        [FromQuery, SwaggerParameter("Page number (default: 1)")] int page = 1,
        [FromQuery, SwaggerParameter("Page size (default: 50, max: 200)")] int pageSize = 50)
    {
        try
        {
            if (pageSize > 200) pageSize = 200;
            if (page < 1) page = 1;

            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(action))
            {
                query = query.Where(l => l.Action == action);
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(l => l.Email.Contains(email));
            }

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(l => l.Role == role);
            }

            if (from.HasValue)
            {
                query = query.Where(l => l.Timestamp >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(l => l.Timestamp <= to.Value);
            }

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new AuditLogResponse
                {
                    Id = l.Id,
                    UserId = l.UserId,
                    Email = l.Email,
                    CompanyName = l.CompanyName,
                    Action = l.Action,
                    Role = l.Role,
                    IpAddress = l.IpAddress,
                    Success = l.Success,
                    Details = l.Details,
                    Timestamp = l.Timestamp
                })
                .ToListAsync();

            return Ok(new AuditLogsListResponse
            {
                Logs = logs,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to fetch audit logs", details = ex.Message });
        }
    }
}
