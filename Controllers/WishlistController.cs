using Microsoft.AspNetCore.Authorization;
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
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WishlistController> _logger;

    public WishlistController(ApplicationDbContext context, ILogger<WishlistController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private int? CurrentUserId => int.TryParse(User.GetUserId(), out var id) ? id : null;

    /// <summary>
    /// Get current user's wishlist (Customer only)
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get current user's wishlist", Description = "Returns all wishlist items for the authenticated customer")]
    [SwaggerResponse(200, "Wishlist retrieved successfully", typeof(WishlistResponse))]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<WishlistResponse>> GetMyWishlist()
    {
        var userId = CurrentUserId;
        if (userId == null)
            return Unauthorized(new { error = "User not authenticated" });

        var items = await _context.Wishlists
            .Where(w => w.UserId == userId.Value)
            .Include(w => w.Product)
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WishlistItemResponse
            {
                Id = w.Id,
                UserId = w.UserId,
                ProductId = w.ProductId,
                ProductName = w.Product!.Name,
                Category = w.Product.Category,
                Images = w.Product.Images,
                Price = w.Product.Price,
                PriceOnRequest = w.Product.PriceOnRequest,
                CreatedAt = w.CreatedAt
            })
            .ToListAsync();

        return Ok(new WishlistResponse { Items = items });
    }

    /// <summary>
    /// Add product to wishlist (Customer only)
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Add product to wishlist", Description = "Adds a product to the authenticated customer's wishlist")]
    [SwaggerResponse(201, "Product added to wishlist", typeof(WishlistItemResponse))]
    [SwaggerResponse(400, "Product already in wishlist or invalid request")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(404, "Product not found")]
    public async Task<ActionResult<WishlistItemResponse>> AddToWishlist([FromBody] AddToWishlistRequest request)
    {
        var userId = CurrentUserId;
        if (userId == null)
            return Unauthorized(new { error = "User not authenticated" });

        // Check if product exists (using string slug PK)
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId);
        if (product == null)
            return NotFound(new { error = "Product not found" });

        // Check if already in wishlist
        var existingItem = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId.Value && w.ProductId == request.ProductId);

        if (existingItem != null)
            return BadRequest(new { error = "Product already in wishlist" });

        // Add to wishlist
        var wishlistItem = new Wishlist
        {
            UserId = userId.Value,
            ProductId = request.ProductId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Wishlists.Add(wishlistItem);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} added product {ProductId} to wishlist", userId, request.ProductId);

        var response = new WishlistItemResponse
        {
            Id = wishlistItem.Id,
            UserId = wishlistItem.UserId,
            ProductId = wishlistItem.ProductId,
            ProductName = product.Name,
            Category = product.Category,
            Images = product.Images,
            Price = product.Price,
            PriceOnRequest = product.PriceOnRequest,
            CreatedAt = wishlistItem.CreatedAt
        };

        return CreatedAtAction(nameof(GetMyWishlist), new { id = wishlistItem.Id }, response);
    }

    /// <summary>
    /// Remove product from wishlist (Customer only)
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remove product from wishlist", Description = "Removes a product from the authenticated customer's wishlist")]
    [SwaggerResponse(200, "Product removed from wishlist")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(404, "Wishlist item not found")]
    public async Task<IActionResult> RemoveFromWishlist(int id)
    {
        var userId = CurrentUserId;
        if (userId == null)
            return Unauthorized(new { error = "User not authenticated" });

        var wishlistItem = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId.Value);

        if (wishlistItem == null)
            return NotFound(new { error = "Wishlist item not found" });

        _context.Wishlists.Remove(wishlistItem);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} removed product {ProductId} from wishlist", userId, wishlistItem.ProductId);

        return Ok(new { success = true, message = "Product removed from wishlist" });
    }

    /// <summary>
    /// Check if a product is in the current user's wishlist
    /// </summary>
    [HttpGet("check/{productId}")]
    [SwaggerOperation(Summary = "Check if product is in wishlist", Description = "Returns whether a product is in the authenticated customer's wishlist")]
    [SwaggerResponse(200, "Check completed")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<object>> CheckWishlist(string productId)
    {
        var userId = CurrentUserId;
        if (userId == null)
            return Unauthorized(new { error = "User not authenticated" });

        var exists = await _context.Wishlists
            .AnyAsync(w => w.UserId == userId.Value && w.ProductId == productId);

        return Ok(new { inWishlist = exists });
    }

    /// <summary>
    /// Get wishlist analytics (Admin only)
    /// </summary>
    [HttpGet("admin/analytics")]
    [AdminOnly]
    [SwaggerOperation(Summary = "Get wishlist analytics", Description = "Returns analytics about customer wishlists including popular products")]
    [SwaggerResponse(200, "Analytics retrieved successfully", typeof(WishlistAnalyticsResponse))]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(403, "Forbidden - Admin only")]
    public async Task<ActionResult<WishlistAnalyticsResponse>> GetWishlistAnalytics()
    {
        try 
        {
            // Get popular products data
            var wishlistGroups = await _context.Wishlists
                .Include(w => w.Product)
                .ToListAsync();

            var popularProducts = wishlistGroups
                .GroupBy(w => new { w.ProductId, Name = w.Product?.Name ?? "Unknown", Category = w.Product?.Category ?? "N/A" })
                .Select(g => new PopularProductResponse
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    Category = g.Key.Category,
                    WishlistCount = g.Count(),
                    InterestedCustomers = _context.Wishlists
                        .Include(w => w.User)
                        .Where(w => w.ProductId == g.Key.ProductId)
                        .Select(w => w.User!.CompanyName ?? w.User.Email)
                        .Distinct()
                        .ToList()
                })
                .OrderByDescending(p => p.WishlistCount)
                .ToList();

            // Get customer summaries
            var customerGroups = await _context.Wishlists
                .Include(w => w.User)
                .Include(w => w.Product)
                .ToListAsync();

            var customerSummaries = customerGroups
                .GroupBy(w => new { w.UserId, Email = w.User?.Email ?? "Unknown", CompanyName = w.User?.CompanyName, ContactPerson = w.User?.ContactPerson })
                .Select(g => new CustomerWishlistSummary
                {
                    UserId = g.Key.UserId,
                    Email = g.Key.Email,
                    CompanyName = g.Key.CompanyName,
                    ContactPerson = g.Key.ContactPerson,
                    WishlistItemCount = g.Count(),
                    Products = g.Select(w => w.Product?.Name ?? "Unknown Product").Distinct().ToList()
                })
                .OrderByDescending(c => c.WishlistItemCount)
                .ToList();

            var totalItems = await _context.Wishlists.CountAsync();
            var uniqueCustomers = await _context.Wishlists.Select(w => w.UserId).Distinct().CountAsync();
            var uniqueProducts = await _context.Wishlists.Select(w => w.ProductId).Distinct().CountAsync();

            return Ok(new WishlistAnalyticsResponse
            {
                PopularProducts = popularProducts,
                CustomerSummaries = customerSummaries,
                TotalWishlistItems = totalItems,
                UniqueCustomers = uniqueCustomers,
                UniqueProducts = uniqueProducts
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting wishlist analytics");
            return StatusCode(500, new { error = "An error occurred while retrieving wishlist analytics", details = ex.Message });
        }
    }

    /// <summary>
    /// Get all customer wishlists (Admin only)
    /// </summary>
    [HttpGet("admin/all")]
    [AdminOnly]
    [SwaggerOperation(Summary = "Get all customer wishlists", Description = "Returns all wishlist items across all customers")]
    [SwaggerResponse(200, "Wishlists retrieved successfully")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(403, "Forbidden - Admin only")]
    public async Task<ActionResult<WishlistResponse>> GetAllWishlists()
    {
        var items = await _context.Wishlists
            .Include(w => w.Product)
            .Include(w => w.User)
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WishlistItemResponse
            {
                Id = w.Id,
                UserId = w.UserId,
                ProductId = w.ProductId,
                ProductName = w.Product!.Name,
                Category = w.Product.Category,
                Images = w.Product.Images,
                Price = w.Product.Price,
                PriceOnRequest = w.Product.PriceOnRequest,
                CreatedAt = w.CreatedAt,
                CustomerEmail = w.User!.Email,
                CustomerCompanyName = w.User.CompanyName,
                CustomerContactPerson = w.User.ContactPerson
            })
            .ToListAsync();

        return Ok(new WishlistResponse { Items = items });
    }
}
