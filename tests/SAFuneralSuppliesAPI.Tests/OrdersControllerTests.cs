using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SAFuneralSuppliesAPI.Data;
using SAFuneralSuppliesAPI.Models;
using Xunit;

namespace SAFuneralSuppliesAPI.Tests;

public class OrdersControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public OrdersControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PlaceOrder_CreatesOrder_ForApprovedCustomer()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("x-test-user-id", "2");
        client.DefaultRequestHeaders.Add("x-test-user-email", "customer@example.com");

        await SeedApprovedCustomerAsync(2, "customer@example.com");

        var payload = new
        {
            items = new[]
            {
                new { productId = "oxford-casket", productName = "Oxford Casket", variant = "Cherry", quantity = 1 }
            },
            notes = "Please confirm availability"
        };

        var response = await client.PostAsJsonAsync("/api/orders", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetMyOrders_ReturnsOnlyCustomerOrders()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("x-test-user-id", "2");
        client.DefaultRequestHeaders.Add("x-test-user-email", "customer@example.com");

        await SeedApprovedCustomerAsync(2, "customer@example.com");
        await SeedApprovedCustomerAsync(3, "other@example.com");

        await SeedOrderAsync(2, "customer@example.com", "Order A");
        await SeedOrderAsync(3, "other@example.com", "Order B");

        var response = await client.GetAsync("/api/orders/my");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var orders = await response.Content.ReadFromJsonAsync<List<Order>>();

        Assert.NotNull(orders);
        Assert.All(orders!, order => Assert.Equal(2, order.CustomerId));
    }

    private async Task SeedApprovedCustomerAsync(int userId, string email)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (db.Users.Any(u => u.Id == userId))
        {
            return;
        }

        db.Users.Add(new User
        {
            Id = userId,
            Email = email,
            Password = "test",
            Role = "customer",
            Status = "approved",
            CompanyName = "Test Co",
            ContactPerson = "Test User"
        });

        await db.SaveChangesAsync();
    }

    private async Task SeedOrderAsync(int userId, string email, string name)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.Orders.Add(new Order
        {
            CustomerId = userId,
            CustomerEmail = email,
            CustomerCompany = "Test Co",
            CustomerContact = name,
            Items = "[]",
            Status = "pending",
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }
}
