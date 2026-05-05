namespace SAFuneralSuppliesAPI.DTOs;

public class ProductExpoDisplayDto
{
    public string Id { get; set; } = string.Empty; // Slug/URL-friendly ID

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public bool PriceOnRequest { get; set; }

    public List<string> Images { get; set; } = new List<string>();

    public List<ColorVariationDto>? ColorVariations { get; set; }
}

public class ColorVariationDto
{
    public string Color { get; set; } = string.Empty;

    public List<string> Images { get; set; } = new List<string>();
}
