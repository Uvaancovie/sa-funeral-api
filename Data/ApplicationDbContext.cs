using Microsoft.EntityFrameworkCore;
using SAFuneralSuppliesAPI.Models;

namespace SAFuneralSuppliesAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<ProductAuditLog> ProductAuditLogs { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fix: SQL Server stores DateTime without timezone info, so EF reads them back
        // with DateTimeKind.Unspecified. This converter tags all DateTime values as UTC
        // so the JSON serializer adds the 'Z' suffix — allowing browsers to correctly
        // convert to local time (e.g. SAST = UTC+2).
        var utcConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );
        var utcNullableConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? (v.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v.Value.ToUniversalTime()) : (DateTime?)null,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null
        );


        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                    property.SetValueConverter(utcConverter);
                else if (property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(utcNullableConverter);
            }
        }

        // Configure AuditLog entity
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.Action);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("now()");
        });

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.Role).HasDefaultValue("customer");
            entity.Property(e => e.Status).HasDefaultValue("pending");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
        });

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.Id).IsUnique();
            entity.Property(e => e.Id).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Category).IsRequired();
            entity.Property(e => e.InStock).HasDefaultValue(true);
            entity.Property(e => e.Featured).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Images).HasDefaultValue("[]");
        });

        // Configure Wishlist entity
        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ProductId);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            // Link ProductId (string slug) to Product.Id (unique principal key)
            entity.HasOne(w => w.Product)
                .WithMany()
                .HasForeignKey(w => w.ProductId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed default admin user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Email = "admin@safuneralsupplies.co.za",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "admin",
                Status = "approved",
                CompanyName = "SA Funeral Supplies",
                ContactPerson = "Admin",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
