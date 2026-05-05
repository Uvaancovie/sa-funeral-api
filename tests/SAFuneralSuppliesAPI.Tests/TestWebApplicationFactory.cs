using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using SAFuneralSuppliesAPI.Configuration;
using SAFuneralSuppliesAPI.Data;
using SAFuneralSuppliesAPI.Services;

namespace SAFuneralSuppliesAPI.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            services.RemoveAll(typeof(BrevoEmailService));
            services.RemoveAll(typeof(BrevoSettings));

            var brevoSettings = new BrevoSettings
            {
                ApiKey = "test",
                SenderEmail = "test@example.com",
                SenderName = "Test",
                MarketingListId = 0,
                CatalogTemplateId = 0,
                CatalogueUrl = "https://example.com/catalog"
            };

            services.AddSingleton(brevoSettings);
            services.AddSingleton(new BrevoEmailService(
                new HttpClient(new OkHttpMessageHandler()),
                brevoSettings,
                NullLogger<BrevoEmailService>.Instance));

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName,
                    _ => { });
        });
    }

    private sealed class OkHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
