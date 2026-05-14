using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using SAFuneralSuppliesAPI.DTOs;

namespace SAFuneralSuppliesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarketingController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public MarketingController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> SubscribeLead([FromBody] LeadCaptureRequest request)
    {
        if (string.IsNullOrEmpty(request.Email))
            return BadRequest(new { error = "Email is required" });

        var apiKey = _configuration["Brevo:ApiKey"];
        var listId = int.Parse(_configuration["Brevo:MarketingListId"] ?? "0");

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("api-key", apiKey);

        // Brevo 'Create Contact' payload format
        var payload = new
        {
            email = request.Email,
            attributes = new { FIRSTNAME = request.FirstName ?? "" },
            listIds = new[] { listId },
            updateEnabled = true // If they already exist, just update them and add to list
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync("https://api.brevo.com/v3/contacts", content);

        if (response.IsSuccessStatusCode)
        {
            return Ok(new { success = true, message = "Subscribed successfully" });
        }

        // Handle error (e.g., read response.Content for Brevo's exact error message)
        return StatusCode(500, new { error = "Failed to subscribe to the mailing list." });
    }
}