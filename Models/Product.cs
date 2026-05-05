using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAFuneralSuppliesAPI.Models;

[Table("Products")]
public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Id { get; set; } = string.Empty; // Slug/URL-friendly ID

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Price { get; set; }

    public bool PriceOnRequest { get; set; }

    public string Images { get; set; } = "[]"; // JSON array

    public string? ColorVariations { get; set; } // JSON array of color variation objects

    public string? Specifications { get; set; } // JSON object

    public string? Features { get; set; } // JSON array

    public bool InStock { get; set; } = true;

    public bool Featured { get; set; }

    public bool ExpoFeatured { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
    
    // Helper properties for deserialization
    [NotMapped]
    public List<string> ImagesList 
    { 
        get => System.Text.Json.JsonSerializer.Deserialize<List<string>>(Images) ?? new List<string>();
        set => Images = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public Dictionary<string, string>? SpecificationsDictionary 
    { 
        get => string.IsNullOrEmpty(Specifications) ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(Specifications);
        set => Specifications = value == null ? null : System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<string>? FeaturesList 
    { 
        get => string.IsNullOrEmpty(Features) ? null : System.Text.Json.JsonSerializer.Deserialize<List<string>>(Features);
        set => Features = value == null ? null : System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<ColorVariation>? ColorVariationsList 
    { 
        get => string.IsNullOrEmpty(ColorVariations) ? null : System.Text.Json.JsonSerializer.Deserialize<List<ColorVariation>>(ColorVariations);
        set => ColorVariations = value == null ? null : System.Text.Json.JsonSerializer.Serialize(value);
    }
}

public class ColorVariation
{
    public string Color { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new List<string>();
}
