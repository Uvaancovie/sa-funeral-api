using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAFuneralSuppliesAPI.Models;

[Table("Orders")]
public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    [MaxLength(255)]
    public string CustomerEmail { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? CustomerCompany { get; set; }

    [MaxLength(255)]
    public string? CustomerContact { get; set; }

    // JSON array: [{ productId, productName, variant, quantity }]
    [Required]
    public string Items { get; set; } = "[]";

    // "pending" | "confirmed" | "processing" | "fulfilled" | "cancelled"
    [MaxLength(50)]
    public string Status { get; set; } = "pending";

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation - not mapped to DB column
    [NotMapped]
    public User? Customer { get; set; }

    [NotMapped]
    public List<OrderItem> ItemsList
    {
        get
        {
            try { return System.Text.Json.JsonSerializer.Deserialize<List<OrderItem>>(Items) ?? new(); }
            catch { return new(); }
        }
    }
}

public class OrderItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Variant { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
}
