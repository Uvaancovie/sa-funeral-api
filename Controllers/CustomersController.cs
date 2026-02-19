using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAFuneralSuppliesAPI.Attributes;
using SAFuneralSuppliesAPI.Data;
using SAFuneralSuppliesAPI.DTOs;
using SAFuneralSuppliesAPI.Extensions;
using SAFuneralSuppliesAPI.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace SAFuneralSuppliesAPI.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
[AdminOnly]
public class CustomersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CustomersController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all customers with optional filtering (Admin only)
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "List all customers",
        Description = "Get all customers with optional status and search filtering (Admin only)",
        Tags = new[] { "Admin - Customers" }
    )]
    [SwaggerResponse(200, "Customers retrieved successfully", typeof(CustomersListResponse))]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(500, "Server error")]
    public async Task<IActionResult> GetCustomers(
        [FromQuery, SwaggerParameter("Filter by status")] string? status, 
        [FromQuery, SwaggerParameter("Search by email, company, or contact")] string? search,
        [FromQuery, SwaggerParameter("Filter by role (admin, customer, all). Defaults to customer.")] string? role = "customer")
    {
        try
        {
            var query = _context.Users.AsQueryable();

            // Default to filtering by customer unless "all" or specific role is requested
            if (string.IsNullOrEmpty(role))
            {
                role = "customer";
            }

            if (role.ToLower() != "all")
            {
                query = query.Where(u => u.Role == role.ToLower());
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(u => u.Status == status);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => 
                    u.Email.Contains(search) || 
                    (u.CompanyName != null && u.CompanyName.Contains(search)) ||
                    (u.ContactPerson != null && u.ContactPerson.Contains(search)));
            }

            var customers = await query
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            var customerResponses = customers.Select(c => new CustomerResponse
            {
                Id = c.Id,
                Email = c.Email,
                CompanyName = c.CompanyName ?? string.Empty,
                ContactPerson = c.ContactPerson ?? string.Empty,
                Phone = c.Phone ?? string.Empty,
                Address = c.Address,
                Status = c.Status,
                Role = c.Role,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList();

            return Ok(new CustomersListResponse { Customers = customerResponses });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to fetch customers", details = ex.Message });
        }
    }

    /// <summary>
    /// Manually create a new customer (Admin only)
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create new customer",
        Description = "Manually create a new customer account (Admin only)",
        Tags = new[] { "Admin - Customers" }
    )]
    [SwaggerResponse(201, "Customer created", typeof(CustomerResponse))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(500, "Server error")]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrEmpty(request.Email) ||
                string.IsNullOrEmpty(request.Password) ||
                string.IsNullOrEmpty(request.CompanyName) ||
                string.IsNullOrEmpty(request.ContactPerson) ||
                string.IsNullOrEmpty(request.Phone))
            {
                return BadRequest(new { error = "Missing required fields" });
            }

            if (request.Password.Length < 8)
            {
                return BadRequest(new { error = "Password must be at least 8 characters" });
            }

            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (existingUser != null)
            {
                return BadRequest(new { error = "Email already exists" });
            }

            // Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create new customer
            var newCustomer = new User
            {
                Email = request.Email.ToLower(),
                Password = hashedPassword,
                CompanyName = request.CompanyName,
                ContactPerson = request.ContactPerson,
                Phone = request.Phone,
                Address = request.Address ?? string.Empty,
                Role = "customer",
                Status = request.Status,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newCustomer);
            await _context.SaveChangesAsync();

            var response = new CustomerResponse
            {
                Id = newCustomer.Id,
                Email = newCustomer.Email,
                CompanyName = newCustomer.CompanyName ?? string.Empty,
                ContactPerson = newCustomer.ContactPerson ?? string.Empty,
                Phone = newCustomer.Phone ?? string.Empty,
                Address = newCustomer.Address,
                Status = newCustomer.Status,
                CreatedAt = newCustomer.CreatedAt
            };

            return CreatedAtAction(nameof(GetCustomers), response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to create customer", details = ex.Message });
        }
    }

    /// <summary>
    /// Update customer status (Admin only)
    /// </summary>
    [HttpPatch("{id}")]
    [SwaggerOperation(
        Summary = "Update customer status",
        Description = "Approve, decline, or set customer status to pending (Admin only)",
        Tags = new[] { "Admin - Customers" }
    )]
    [SwaggerResponse(200, "Status updated successfully")]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(404, "Customer not found")]
    [SwaggerResponse(500, "Server error")]
    public async Task<IActionResult> UpdateCustomerStatus(
        [SwaggerParameter("Customer ID")] int id, 
        [FromBody] UpdateCustomerStatusRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Status) ||
                !new[] { "approved", "declined", "pending" }.Contains(request.Status))
            {
                return BadRequest(new { error = "Valid status required (approved, declined, pending)" });
            }

            var adminUserId = User.GetUserId();
            int? adminId = string.IsNullOrEmpty(adminUserId) ? null : int.Parse(adminUserId);

            var customer = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.Role == "customer");

            if (customer == null)
            {
                return NotFound(new { error = "Customer not found" });
            }

            customer.Status = request.Status;
            customer.UpdatedAt = DateTime.UtcNow;
            customer.UpdatedBy = adminId;

            if (!string.IsNullOrEmpty(request.Reason))
            {
                customer.StatusReason = request.Reason;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"Customer {request.Status} successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Update failed", details = ex.Message });
        }
    }

    /// <summary>
    /// Update customer details (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update customer details",
        Description = "Update full customer information including company, contact, phone, email, and address (Admin only)",
        Tags = new[] { "Admin - Customers" }
    )]
    [SwaggerResponse(200, "Customer updated successfully")]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(404, "Customer not found")]
    [SwaggerResponse(500, "Server error")]
    public async Task<IActionResult> UpdateCustomer(
        [SwaggerParameter("Customer ID")] int id,
        [FromBody] UpdateCustomerRequest request)
    {
        try
        {
            var customer = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.Role == "customer");

            if (customer == null)
            {
                return NotFound(new { error = "Customer not found" });
            }

            // Check if email is being changed and if it's already taken
            if (!string.IsNullOrEmpty(request.Email) && request.Email.ToLower() != customer.Email.ToLower())
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (existingUser != null)
                {
                    return BadRequest(new { error = "Email already exists" });
                }

                customer.Email = request.Email.ToLower();
            }

            if (!string.IsNullOrEmpty(request.CompanyName))
                customer.CompanyName = request.CompanyName;

            if (!string.IsNullOrEmpty(request.ContactPerson))
                customer.ContactPerson = request.ContactPerson;

            if (!string.IsNullOrEmpty(request.Phone))
                customer.Phone = request.Phone;

            if (request.Address != null)
                customer.Address = request.Address;

            if (!string.IsNullOrEmpty(request.Status) &&
                new[] { "approved", "declined", "pending" }.Contains(request.Status))
            {
                customer.Status = request.Status;
            }

            var adminUserId = User.GetUserId();
            customer.UpdatedAt = DateTime.UtcNow;
            customer.UpdatedBy = string.IsNullOrEmpty(adminUserId) ? null : int.Parse(adminUserId);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Customer updated successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Update failed", details = ex.Message });
        }
    }

    /// <summary>
    /// Delete a customer (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete customer",
        Description = "Permanently delete a customer account (Admin only)",
        Tags = new[] { "Admin - Customers" }
    )]
    [SwaggerResponse(200, "Customer deleted successfully")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(404, "Customer not found")]
    [SwaggerResponse(500, "Server error")]
    public async Task<IActionResult> DeleteCustomer([SwaggerParameter("Customer ID")] int id)
    {
        try
        {
            var customer = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.Role == "customer");

            if (customer == null)
            {
                return NotFound(new { error = "Customer not found" });
            }

            _context.Users.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Customer deleted successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Delete failed", details = ex.Message });
        }
    }

    /// <summary>
    /// Bulk update customer status (Admin only)
    /// </summary>
    [HttpPost("bulk-update")]
    [SwaggerOperation(
        Summary = "Bulk update customer status",
        Description = "Update status for multiple customers at once (Admin only)",
        Tags = new[] { "Admin - Customers" }
    )]
    [SwaggerResponse(200, "Customers updated successfully")]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(500, "Server error")]
    public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateRequest request)
    {
        try
        {
            if (request.CustomerIds == null || !request.CustomerIds.Any())
            {
                return BadRequest(new { error = "Customer IDs required" });
            }

            if (string.IsNullOrEmpty(request.Status) ||
                !new[] { "approved", "declined", "pending" }.Contains(request.Status))
            {
                return BadRequest(new { error = "Valid status required (approved, declined, pending)" });
            }

            var adminUserId = User.GetUserId();
            int? adminId = string.IsNullOrEmpty(adminUserId) ? null : int.Parse(adminUserId);

            var customers = await _context.Users
                .Where(u => request.CustomerIds.Contains(u.Id) && u.Role == "customer")
                .ToListAsync();

            foreach (var customer in customers)
            {
                customer.Status = request.Status;
                customer.UpdatedAt = DateTime.UtcNow;
                customer.UpdatedBy = adminId;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"{customers.Count} customer(s) updated successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Bulk update failed", details = ex.Message });
        }
    }

    /// <summary>
    /// Bulk delete customers (Admin only)
    /// </summary>
    [HttpPost("bulk-delete")]
    [SwaggerOperation(
        Summary = "Bulk delete customers",
        Description = "Delete multiple customers at once (Admin only)",
        Tags = new[] { "Admin - Customers" }
    )]
    [SwaggerResponse(200, "Customers deleted successfully")]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(500, "Server error")]
    public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteRequest request)
    {
        try
        {
            if (request.CustomerIds == null || !request.CustomerIds.Any())
            {
                return BadRequest(new { error = "Customer IDs required" });
            }

            var customers = await _context.Users
                .Where(u => request.CustomerIds.Contains(u.Id) && u.Role == "customer")
                .ToListAsync();

            _context.Users.RemoveRange(customers);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"{customers.Count} customer(s) deleted successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Bulk delete failed", details = ex.Message });
        }
    }


    /// <summary>
    /// Update user role (Promote/Demote) (Admin only)
    /// </summary>
    [HttpPatch("{id}/role")]
    [SwaggerOperation(
        Summary = "Update user role",
        Description = "Promote a customer to admin or demote an admin to customer. Logs the event.",
        Tags = new[] { "Admin - Customers" }
    )]
    [SwaggerResponse(200, "Role updated successfully")]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(403, "Forbidden - Cannot demote self or super admin")]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> UpdateUserRole(
        [SwaggerParameter("User ID")] int id,
        [FromBody] UpdateRoleRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Role) || 
                !new[] { "admin", "customer" }.Contains(request.Role.ToLower()))
            {
                return BadRequest(new { error = "Valid role required (admin, customer)" });
            }

            var currentUserId = User.GetUserId();
            int currentAdminId = string.IsNullOrEmpty(currentUserId) ? 0 : int.Parse(currentUserId);

            // Prevent changing own role
            if (id == currentAdminId)
            {
                return BadRequest(new { error = "You cannot change your own role." });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            // Prevent demoting the primary admin (hardcoded safety check for demonstration)
            if (user.Email.ToLower() == "admin@safuneralsupplies.co.za" && request.Role.ToLower() != "admin")
            {
                 return StatusCode(403, new { error = "Cannot demote the primary system administrator." });
            }
            
            var oldRole = user.Role;
            user.Role = request.Role.ToLower();
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = currentAdminId;

            await _context.SaveChangesAsync();

            // Log audit event
            var auditLog = new AuditLog
            {
                UserId = currentAdminId,
                Email = User.GetEmail() ?? "unknown",
                Action = "update_role",
                Role = "admin",
                Details = $"Changed role of user {user.Email} (ID: {user.Id}) from {oldRole} to {user.Role}",
                Timestamp = DateTime.UtcNow,
                Success = true,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"User role updated to {user.Role}",
                userId = user.Id,
                newRole = user.Role
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to update role", details = ex.Message });
        }
    }
}
