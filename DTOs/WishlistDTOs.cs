namespace SAFuneralSuppliesAPI.DTOs;

public class WishlistItemResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ProductId { get; set; } = string.Empty; // Product slug
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Images { get; set; }
    public decimal? Price { get; set; }
    public bool PriceOnRequest { get; set; }
    public DateTime CreatedAt { get; set; }

    // Customer info (for admin view)
    public string? CustomerEmail { get; set; }
    public string? CustomerCompanyName { get; set; }
    public string? CustomerContactPerson { get; set; }
}

public class AddToWishlistRequest
{
    public string ProductId { get; set; } = string.Empty; // Product slug
}

public class WishlistResponse
{
    public List<WishlistItemResponse> Items { get; set; } = new();
}

public class WishlistAnalyticsResponse
{
    public List<PopularProductResponse> PopularProducts { get; set; } = new();
    public List<CustomerWishlistSummary> CustomerSummaries { get; set; } = new();
    public int TotalWishlistItems { get; set; }
    public int UniqueCustomers { get; set; }
    public int UniqueProducts { get; set; }
}

public class PopularProductResponse
{
    public string ProductId { get; set; } = string.Empty; // Product slug
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int WishlistCount { get; set; }
    public List<string> InterestedCustomers { get; set; } = new();
}

public class CustomerWishlistSummary
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? ContactPerson { get; set; }
    public int WishlistItemCount { get; set; }
    public List<string> Products { get; set; } = new();
}
