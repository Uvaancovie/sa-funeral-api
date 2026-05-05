using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAFuneralSuppliesAPI.Attributes;
using SAFuneralSuppliesAPI.Data;
using SAFuneralSuppliesAPI.DTOs;
using SAFuneralSuppliesAPI.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace SAFuneralSuppliesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetCurrentUserEmail()
    {
        var email = _httpContextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
        return email ?? "Unknown Admin";
    }

    private void LogProductChange(string productId, string productName, string action, object? changes = null)
    {
        try 
        {
            var auditLog = new ProductAuditLog
            {
                ProductId = productId,
                ProductName = productName,
                Action = action,
                ChangedBy = GetCurrentUserEmail(),
                Timestamp = DateTime.UtcNow,
                Changes = changes != null ? System.Text.Json.JsonSerializer.Serialize(changes) : null
            };
            
            _context.ProductAuditLogs.Add(auditLog);
            // Not calling SaveChangesAsync here as it will be called by the main action or we can call it if needed separate
        }
        catch (Exception ex)
        {
            // Fail silently to not impact main flow, but log error
            Console.WriteLine($"Failed to create audit log: {ex.Message}");
        }
    }

    /// <summary>
    /// Get all products with optional filtering
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "List all products",
        Description = "Get all products with optional category and search filtering",
        Tags = new[] { "Products" }
    )]
    [SwaggerResponse(200, "Products retrieved successfully", typeof(List<Product>))]
    [SwaggerResponse(500, "Server error")]
    public async Task<IActionResult> GetProducts(
        [FromQuery, SwaggerParameter("Filter by category")] string? category, 
        [FromQuery, SwaggerParameter("Search by name or ID")] string? search)
    {
        try
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => 
                    p.Name.Contains(search) || 
                    p.Id.Contains(search));
            }

            var products = await query.ToListAsync();

            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to fetch products", details = ex.Message });
        }
    }

    /// <summary>
    /// Get a single product by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get product by ID",
        Description = "Retrieve a single product by its slug/ID",
        Tags = new[] { "Products" }
    )]
    [SwaggerResponse(200, "Product found", typeof(Product))]
    [SwaggerResponse(404, "Product not found")]
    [SwaggerResponse(500, "Server error")]
    public async Task<IActionResult> GetProduct([SwaggerParameter("Product slug/ID")] string id)
    {
        try
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound(new { error = "Product not found" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to fetch product", details = ex.Message });
        }
    }

    /// <summary>
    /// Get all products marked for expo display
    /// </summary>
    [HttpGet("expo")]
    [SwaggerOperation(
        Summary = "Get expo featured products",
        Description = "Get all products marked for expo/kiosk display (public endpoint)",
        Tags = new[] { "Products" }
    )]
    [SwaggerResponse(200, "Expo products retrieved successfully", typeof(List<ProductExpoDisplayDto>))]
    [SwaggerResponse(500, "Server error")]
    public async Task<IActionResult> GetExpoProducts()
    {
        try
        {
            // First, fetch the products from database
            var dbProducts = await _context.Products
                .Where(p => p.ExpoFeatured && p.InStock)
                .OrderBy(p => p.Name)
                .ToListAsync();

            // Then, transform to DTO in memory (to allow JSON deserializing)
            var expoProducts = dbProducts.Select(p => new ProductExpoDisplayDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Description = p.Description,
                Price = p.Price,
                PriceOnRequest = p.PriceOnRequest,
                Images = System.Text.Json.JsonSerializer.Deserialize<List<string>>(p.Images) ?? new List<string>(),
                ColorVariations = string.IsNullOrEmpty(p.ColorVariations) ? null :
                    System.Text.Json.JsonSerializer.Deserialize<List<ColorVariationDto>>(p.ColorVariations)
            }).ToList();

            return Ok(expoProducts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to fetch expo products", details = ex.Message });
        }
    }

    /// <summary>
    /// Create a new product (Admin only)
    /// </summary>
    [HttpPost]
    [AdminOnly]
    [SwaggerOperation(
        Summary = "Create new product",
        Description = "Create a new product (Admin only)",
        Tags = new[] { "Products - Admin" }
    )]
    [SwaggerResponse(201, "Product created", typeof(Product))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(409, "Product ID already exists")]
    [SwaggerResponse(500, "Server error")]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        try
        {
            if (string.IsNullOrEmpty(product.Id) || string.IsNullOrEmpty(product.Name))
            {
                return BadRequest(new { error = "Product ID and name are required" });
            }

            // Check if product with same ID already exists
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            if (existingProduct != null)
            {
                return Conflict(new { error = "Product with this ID already exists" });
            }

            product.CreatedAt = DateTime.UtcNow;
            _context.Products.Add(product);
            
            // Log creation
            LogProductChange(product.Id, product.Name, "Create", product);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to create product", details = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing product (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [AdminOnly]
    [SwaggerOperation(
        Summary = "Update product",
        Description = "Update an existing product (Admin only)",
        Tags = new[] { "Products - Admin" }
    )]
    [SwaggerResponse(200, "Product updated", typeof(Product))]
    [SwaggerResponse(404, "Product not found")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(500, "Server error")]
    public async Task<IActionResult> UpdateProduct(
        [SwaggerParameter("Product slug/ID")] string id, 
        [FromBody] Product updates)
    {
        try
        {
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProduct == null)
            {
                return NotFound(new { error = "Product not found" });
            }

            // Calculate changes
            var changes = new Dictionary<string, object>();
            if (existingProduct.Name != updates.Name) changes["Name"] = new { old = existingProduct.Name, @new = updates.Name };
            if (existingProduct.Price != updates.Price) changes["Price"] = new { old = existingProduct.Price, @new = updates.Price };
            if (existingProduct.InStock != updates.InStock) changes["InStock"] = new { old = existingProduct.InStock, @new = updates.InStock };
            if (existingProduct.PriceOnRequest != updates.PriceOnRequest) changes["PriceOnRequest"] = new { old = existingProduct.PriceOnRequest, @new = updates.PriceOnRequest };
            // Add other significant fields as needed

            // Update fields
            existingProduct.Name = updates.Name;
            existingProduct.Category = updates.Category;
            existingProduct.Description = updates.Description;
            existingProduct.Price = updates.Price;
            existingProduct.PriceOnRequest = updates.PriceOnRequest;
            existingProduct.Images = updates.Images;
            existingProduct.Specifications = updates.Specifications;
            existingProduct.Features = updates.Features;
            existingProduct.ColorVariations = updates.ColorVariations;
            existingProduct.InStock = updates.InStock;
            existingProduct.Featured = updates.Featured;
            existingProduct.ExpoFeatured = updates.ExpoFeatured;
            existingProduct.UpdatedAt = DateTime.UtcNow;
            
            if (changes.Count > 0)
            {
                LogProductChange(existingProduct.Id, existingProduct.Name, "Update", changes);
            }

            await _context.SaveChangesAsync();

            return Ok(existingProduct);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to update product", details = ex.Message });
        }
    }

    /// <summary>
    /// Delete a product (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [AdminOnly]
    [SwaggerOperation(
        Summary = "Delete product",
        Description = "Delete a product (Admin only)",
        Tags = new[] { "Products - Admin" }
    )]
    [SwaggerResponse(200, "Product deleted")]
    [SwaggerResponse(404, "Product not found")]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(500, "Server error")]
    public async Task<IActionResult> DeleteProduct([SwaggerParameter("Product slug/ID")] string id)
    {
        try
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound(new { error = "Product not found" });
            }

            _context.Products.Remove(product);
            
            LogProductChange(product.Id, product.Name, "Delete");
            
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Product deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to delete product", details = ex.Message });
        }
    }

    /// <summary>
    /// Get history/audit logs for a specific product (Admin only)
    /// </summary>
    [HttpGet("{id}/history")]
    [AdminOnly]
    [SwaggerOperation(
        Summary = "Get product history",
        Description = "Retrieve audit logs for a specific product",
        Tags = new[] { "Products - Admin" }
    )]
    [SwaggerResponse(200, "History retrieved", typeof(List<ProductAuditLog>))]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<IActionResult> GetProductHistory([SwaggerParameter("Product slug/ID")] string id)
    {
        try
        {
            var logs = await _context.ProductAuditLogs
                .Where(l => l.ProductId == id)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();

            return Ok(logs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to fetch product history", details = ex.Message });
        }
    }
}
