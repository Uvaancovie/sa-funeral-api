using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAFuneralSuppliesAPI.Attributes;
using SAFuneralSuppliesAPI.Data;
using SAFuneralSuppliesAPI.Models;
using SAFuneralSuppliesAPI.Services;
using System.Security.Claims;

namespace SAFuneralSuppliesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly BrevoEmailService _emailService;

    public OrdersController(ApplicationDbContext context, BrevoEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    private int? GetCurrentUserId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }

    private string GetCurrentUserEmail() =>
        User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "Unknown";

    /// <summary>Place a new order (approved customers only)</summary>
    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized(new { error = "Not authenticated" });

        var user = await _context.Users.FindAsync(userId.Value);
        if (user == null) return Unauthorized(new { error = "User not found" });
        if (user.Status != "approved" && user.Role != "admin")
            return Forbid();

        if (request.Items == null || !request.Items.Any())
            return BadRequest(new { error = "Order must contain at least one item" });

        var camelCase = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        };

        var order = new Order
        {
            CustomerId = userId.Value,
            CustomerEmail = user.Email,
            CustomerCompany = user.CompanyName,
            CustomerContact = user.ContactPerson,
            Items = System.Text.Json.JsonSerializer.Serialize(request.Items, camelCase),
            Notes = request.Notes,
            Status = "pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Send confirmation email asynchronously
        _ = _emailService.SendOrderConfirmationAsync(order, user.CompanyName ?? user.Email);
        _ = _emailService.AddContactToMarketingListAsync(user.Email, user.ContactPerson ?? user.CompanyName ?? "Customer", user.Phone ?? string.Empty);
        _ = _emailService.SendCatalogEmailAsync(user.Email, user.ContactPerson ?? user.CompanyName ?? "Customer");

        return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
    }

    /// <summary>Get a single order by ID</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        // Only admin or the order owner can view it
        var user = await _context.Users.FindAsync(userId.Value);
        if (user?.Role != "admin" && order.CustomerId != userId.Value)
            return Forbid();

        return Ok(order);
    }

    /// <summary>Get all orders — admin only</summary>
    [HttpGet]
    [AdminOnly]
    public async Task<IActionResult> GetAllOrders(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _context.Orders.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(o => o.Status == status);

        var total = await query.CountAsync();
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { total, page, pageSize, orders });
    }

    /// <summary>Get current customer's own orders</summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var orders = await _context.Orders
            .Where(o => o.CustomerId == userId.Value)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return Ok(orders);
    }

    /// <summary>Update order status — admin only</summary>
    [HttpPut("{id:int}/status")]
    [AdminOnly]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var validStatuses = new[] { "pending", "confirmed", "processing", "fulfilled", "cancelled" };
        if (!validStatuses.Contains(request.Status))
            return BadRequest(new { error = $"Invalid status. Must be one of: {string.Join(", ", validStatuses)}" });

        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.Status = request.Status;
        order.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(order);
    }
}

public class PlaceOrderRequest
{
    public List<OrderItemRequest> Items { get; set; } = new();
    public string? Notes { get; set; }
}

public class OrderItemRequest
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Variant { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
