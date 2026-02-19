using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SAFuneralSuppliesAPI.Configuration;
using SAFuneralSuppliesAPI.Data;
using SAFuneralSuppliesAPI.Services;
using SAFuneralSuppliesAPI.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection (Postgres / Supabase)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not configured");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure JWT settings
var jwtSettings = new JwtSettings
{
    Secret = builder.Configuration["Jwt:Secret"]
        ?? throw new InvalidOperationException("JWT secret not configured"),
    Issuer = builder.Configuration["Jwt:Issuer"] ?? "SAFuneralSuppliesAPI",
    Audience = builder.Configuration["Jwt:Audience"] ?? "SAFuneralSuppliesApp",
    ExpirationDays = int.Parse(builder.Configuration["Jwt:ExpirationDays"] ?? "7")
};

// Add services to container
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<JwtService>();

// Add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Enhanced Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SA Funeral Supplies API",
        Version = "v1",
        Description = @"
## 🎉 Welcome to SA Funeral Supplies API!

A comprehensive REST API for managing funeral supplies products and customer accounts.

### 🔑 Quick Start with Authentication:
1. **Login:** Use `POST /api/auth/login` with default credentials:
   - Email: `admin@safuneralsupplies.co.za`
   - Password: `Admin123!`
2. **Copy the token** from the response
3. **Click 'Authorize' button** (🔒 icon at top right)
4. **Enter:** `Bearer your-token-here`
5. **Now you can test all endpoints!**

### 📚 API Categories:
- **Authentication:** Login and user management
- **Products:** Browse and search products (public)
- **Products - Admin:** Create, update, delete products
- **Admin - Customers:** Manage customer accounts

### 🛠️ Features:
- JWT Bearer authentication
- Role-based authorization (Admin/Customer)
- SQL Server database with Entity Framework Core
- RESTful API design
- Comprehensive validation

### 💡 Tips:
- All admin endpoints require authentication
- Product listing and detail views are public
- Use the search and filter parameters for better results
",
        Contact = new OpenApiContact
        {
            Name = "SA Funeral Supplies",
            Email = "admin@safuneralsupplies.co.za"
        }
    });

    // Enable annotations for better documentation
    c.EnableAnnotations();

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"JWT Authorization header using the Bearer scheme. 

Enter your JWT token in the text input below.

Example: '12345abcdef'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Group endpoints by tags
    c.TagActionsBy(api =>
    {
        if (api.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor descriptor)
        {
            var swaggerOperation = descriptor.MethodInfo
                .GetCustomAttributes(typeof(Swashbuckle.AspNetCore.Annotations.SwaggerOperationAttribute), false)
                .FirstOrDefault() as Swashbuckle.AspNetCore.Annotations.SwaggerOperationAttribute;

            if (swaggerOperation?.Tags?.Any() == true)
            {
                return swaggerOperation.Tags;
            }
        }
        return new[] { api.GroupName ?? "Default" };
    });
    c.DocInclusionPredicate((name, api) => true);

    // Order tags
    c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
});

var app = builder.Build();

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        db.Database.Migrate();
        Console.WriteLine("✓ Database migrated successfully");
        
        // Seed products if database is empty
        // Use the project root directory (parent of SAFuneralSuppliesAPI folder)
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var productsJsonPath = Path.Combine(projectRoot, "products.json");
        var assetsPath = Path.Combine(projectRoot, "src", "assets");
        
        Console.WriteLine($"Looking for products.json at: {productsJsonPath}");
        Console.WriteLine($"Looking for assets at: {assetsPath}");
        
        if (File.Exists(productsJsonPath))
        {
            await ProductSeeder.SeedProducts(db, productsJsonPath, assetsPath);
        }
        else
        {
            Console.WriteLine("⚠ products.json not found. Skipping product seeding.");
            Console.WriteLine($"   Expected: {productsJsonPath}");
            Console.WriteLine($"   Current directory: {Directory.GetCurrentDirectory()}");
            Console.WriteLine($"   Base directory: {AppContext.BaseDirectory}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠ Warning: Database migration failed - {ex.Message}");
        Console.WriteLine("  Make sure SQL Server is running and connection string is correct.");
    }
}

// Configure middleware pipeline - Swagger always enabled for easy testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SA Funeral Supplies API v1");
    c.RoutePrefix = string.Empty; // Swagger at root URL
    c.DocumentTitle = "SA Funeral Supplies API - Interactive Documentation";
    c.DefaultModelsExpandDepth(2);
    c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example);
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.EnableFilter();
    c.ShowExtensions();
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Display startup information
Console.WriteLine("\n========================================");
Console.WriteLine("✨ SA Funeral Supplies API Started!");
Console.WriteLine("========================================");
Console.WriteLine($"🌐 Swagger UI: {(app.Environment.IsDevelopment() ? "http://localhost:5000" : "https://localhost:5001")}");
Console.WriteLine("🔐 Default Admin Credentials:");
Console.WriteLine("   Email: admin@safuneralsupplies.co.za");
Console.WriteLine("   Password: Admin123!");
Console.WriteLine("========================================\n");

app.Run();
