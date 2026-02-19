using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SAFuneralSuppliesAPI.Data;
using SAFuneralSuppliesAPI.Models;

namespace SAFuneralSuppliesAPI.Seeders;

public class ProductSeeder
{
    public static async Task SeedProducts(ApplicationDbContext context, string productsJsonPath, string assetsPath)
    {
        // Check if products already exist
        if (await context.Products.AnyAsync())
        {
            Console.WriteLine("Products already exist in database. Skipping seed.");
            return;
        }

        Console.WriteLine("Reading products.json...");
        var jsonContent = await File.ReadAllTextAsync(productsJsonPath);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var productsJson = JsonSerializer.Deserialize<List<ProductJson>>(jsonContent, jsonOptions);

        if (productsJson == null || !productsJson.Any())
        {
            Console.WriteLine("No products found in products.json");
            return;
        }

        Console.WriteLine($"Found {productsJson.Count} products to seed");

        int addedCount = 0;
        foreach (var productJson in productsJson)
        {
            try
            {
                // Skip products with empty or null IDs
                if (string.IsNullOrWhiteSpace(productJson.Id))
                {
                    Console.WriteLine($"  ⏭  Skipping product with empty ID: {productJson.Name}");
                    continue;
                }

                // Get all images for this product
                var productFolder = Path.Combine(assetsPath, productJson.Id);
                var images = new List<string>();

                if (Directory.Exists(productFolder))
                {
                    var imageFiles = Directory.GetFiles(productFolder, "*.png")
                        .Concat(Directory.GetFiles(productFolder, "*.jpg"))
                        .Concat(Directory.GetFiles(productFolder, "*.jpeg"))
                        .OrderBy(f => f)
                        .ToList();

                    foreach (var imagePath in imageFiles)
                    {
                        var relativePath = $"assets/{productJson.Id}/{Path.GetFileName(imagePath)}";
                        images.Add(relativePath);
                    }
                }
                else
                {
                    // Fallback to the single image from JSON
                    images.Add(productJson.Image);
                }

                // Create product
                var product = new Product
                {
                    Id = productJson.Id,
                    Name = productJson.Name,
                    Category = productJson.Category,
                    Description = $"{productJson.Name} available in {string.Join(", ", productJson.Variants)}",
                    Price = productJson.Price > 0 ? productJson.Price : null,
                    PriceOnRequest = productJson.Price == 0,
                    Images = JsonSerializer.Serialize(images),
                    Features = JsonSerializer.Serialize(productJson.Variants),
                    Specifications = JsonSerializer.Serialize(new Dictionary<string, string>
                    {
                        { "Available Variants", string.Join(", ", productJson.Variants) },
                        { "Category", CapitalizeCategory(productJson.Category) }
                    }),
                    ColorVariations = CreateColorVariations(productJson.Variants, images),
                    InStock = true,
                    Featured = false,
                    CreatedAt = DateTime.UtcNow
                };

                context.Products.Add(product);
                addedCount++;
                Console.WriteLine($"  ✓ Added: {product.Name} ({images.Count} images)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Error adding {productJson.Name}: {ex.Message}");
            }
        }

        await context.SaveChangesAsync();
        Console.WriteLine($"Successfully seeded {addedCount} out of {productsJson.Count} products!");
    }

    private static string CapitalizeCategory(string category)
    {
        return category switch
        {
            "casket" => "Casket",
            "accessory" => "Accessory",
            "child" => "Child Casket",
            _ => category
        };
    }

    private static string? CreateColorVariations(List<string> variants, List<string> images)
    {
        // Only create color variations if we have actual color names in variants
        var colorKeywords = new[] { "cherry", "teak", "kiaat", "walnut", "white", "ash", "black", "brown", "green", "hemlock", "oak", "mahogany", "pine" };
        var colorVariants = variants
            .Where(v => colorKeywords.Any(k => v.ToLower().Contains(k)))
            .ToList();

        if (!colorVariants.Any() || !images.Any())
        {
            return null;
        }

        // Create color variation objects - distribute images across colors
        var variations = new List<ColorVariation>();
        var imagesPerColor = Math.Max(1, images.Count / Math.Max(1, colorVariants.Count));

        for (int i = 0; i < colorVariants.Count; i++)
        {
            var colorImages = images.Skip(i * imagesPerColor).Take(imagesPerColor).ToList();
            if (!colorImages.Any())
            {
                colorImages = new List<string> { images.First() }; // Fallback to first image
            }

            variations.Add(new ColorVariation
            {
                Color = colorVariants[i],
                Images = colorImages
            });
        }

        return variations.Any() ? JsonSerializer.Serialize(variations) : null;
    }

    private class ProductJson
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public List<string> Variants { get; set; } = new();
        public decimal Price { get; set; }
    }
}
