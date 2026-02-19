using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAFuneralSuppliesAPI.Data;
using SAFuneralSuppliesAPI.DTOs;
using SAFuneralSuppliesAPI.Extensions;
using SAFuneralSuppliesAPI.Models;
using SAFuneralSuppliesAPI.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SAFuneralSuppliesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(ApplicationDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "User login",
        Description = "Authenticate with email and password. Returns JWT token.",
        Tags = new[] { "Authentication" }
    )]
    [SwaggerResponse(200, "Login successful", typeof(LoginResponse))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(401, "Invalid credentials")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers["User-Agent"].ToString();

        try
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { error = "Email and password required" });
            }

            // Find user by email (case-insensitive)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
            {
                // Log failed login attempt
                await LogAuditEvent(0, request.Email, null, "login_failed", "unknown", ipAddress, userAgent, false, "User not found");
                return Unauthorized(new { error = "Invalid credentials" });
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                await LogAuditEvent(user.Id, user.Email, user.CompanyName, "login_failed", user.Role, ipAddress, userAgent, false, "No password set");
                return Unauthorized(new { error = "Invalid credentials" });
            }

            // Verify password
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            
            if (!isValidPassword)
            {
                await LogAuditEvent(user.Id, user.Email, user.CompanyName, "login_failed", user.Role, ipAddress, userAgent, false, "Invalid password");
                return Unauthorized(new { error = "Invalid credentials" });
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            // Log successful login
            await LogAuditEvent(user.Id, user.Email, user.CompanyName, "login", user.Role, ipAddress, userAgent, true, null);

            var response = new LoginResponse
            {
                Token = token,
                User = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.Role,
                    Status = user.Status,
                    CompanyName = user.CompanyName,
                    ContactPerson = user.ContactPerson,
                    Phone = user.Phone,
                    Address = user.Address
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Login failed", details = ex.Message });
        }
    }

    /// <summary>
    /// Logout the current user (logs the event for audit compliance)
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [SwaggerOperation(
        Summary = "User logout",
        Description = "Logout the current user. Records the logout event in audit logs for compliance.",
        Tags = new[] { "Authentication" }
    )]
    [SwaggerResponse(200, "Logout successful")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userId = User.GetUserId();
            var email = User.GetEmail();
            var role = User.GetRole();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();

            // Find user to get company name
            string? companyName = null;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _context.Users.FindAsync(int.Parse(userId));
                companyName = user?.CompanyName;
            }

            await LogAuditEvent(
                string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId),
                email ?? "unknown",
                companyName,
                "logout",
                role ?? "unknown",
                ipAddress,
                userAgent,
                true,
                null
            );

            return Ok(new { success = true, message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Logout failed", details = ex.Message });
        }
    }

    /// <summary>
    /// Register endpoint - disabled for security
    /// </summary>
    [HttpPost("register")]
    [SwaggerOperation(
        Summary = "Public registration (disabled)",
        Description = "Public registration is disabled. Customers must be created by admin.",
        Tags = new[] { "Authentication" }
    )]
    [SwaggerResponse(410, "Registration disabled")]
    public IActionResult Register()
    {
        return StatusCode(410, new
        {
            error = "Public registration is disabled. Customer accounts are created by admin only."
        });
    }

    private async Task LogAuditEvent(int userId, string email, string? companyName, string action, string role, string? ipAddress, string? userAgent, bool success, string? details)
    {
        try
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Email = email,
                CompanyName = companyName,
                Action = action,
                Role = role,
                IpAddress = ipAddress,
                UserAgent = userAgent?.Length > 500 ? userAgent[..500] : userAgent,
                Success = success,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Don't let audit logging failures break the auth flow
            Console.WriteLine($"⚠ Audit log failed: {ex.Message}");
        }
    }
}
