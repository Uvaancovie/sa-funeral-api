using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SAFuneralSuppliesAPI.Configuration;
using SAFuneralSuppliesAPI.Models;

namespace SAFuneralSuppliesAPI.Services;

public class BrevoEmailService
{
    private readonly HttpClient _httpClient;
    private readonly BrevoSettings _settings;
    private readonly ILogger<BrevoEmailService> _logger;

    public BrevoEmailService(HttpClient httpClient, BrevoSettings settings, ILogger<BrevoEmailService> logger)
    {
        _httpClient = httpClient;
        _settings = settings;
        _logger = logger;
        
        _httpClient.BaseAddress = new Uri("https://api.brevo.com/v3/");
        _httpClient.DefaultRequestHeaders.Add("api-key", _settings.ApiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task SendOrderConfirmationAsync(Order order, string customerName)
    {
        try
        {
            var itemsHtml = new StringBuilder();
            foreach (var item in order.ItemsList)
            {
                itemsHtml.AppendLine($@"
                    <tr>
                        <td style='padding: 10px; border-bottom: 1px solid #ddd;'>{item.ProductName}</td>
                        <td style='padding: 10px; border-bottom: 1px solid #ddd; color: #555;'>{item.Variant}</td>
                        <td style='padding: 10px; border-bottom: 1px solid #ddd; text-align: center; font-weight: bold;'>{item.Quantity}</td>
                    </tr>");
            }

            var htmlContent = $@"
            <html>
                <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
                    <div style='background: #1a103c; padding: 20px; text-align: center;'>
                        <h2 style='color: #a89f6e; margin: 0;'>SA FUNERAL SUPPLIES</h2>
                    </div>
                    <div style='padding: 20px;'>
                        <h3>Order Confirmation</h3>
                        <p>Dear {customerName},</p>
                        <p>Thank you for placing your order with us. Your order status is currently <strong>Pending</strong>.</p>
                        <p><strong>Order ID:</strong> #{order.OrderId.ToString().PadLeft(5, '0')}</p>
                        
                        <table style='width: 100%; border-collapse: collapse; margin-top: 20px; font-size: 14px;'>
                            <thead>
                                <tr style='background: #f8f9fa; text-transform: uppercase; font-size: 12px;'>
                                    <th style='padding: 10px; text-align: left;'>Product</th>
                                    <th style='padding: 10px; text-align: left;'>Variant</th>
                                    <th style='padding: 10px;'>Qty</th>
                                </tr>
                            </thead>
                            <tbody>
                                {itemsHtml}
                            </tbody>
                        </table>
                        
                        {(string.IsNullOrEmpty(order.Notes) ? "" : $"<p style='margin-top: 20px;'><strong>Your Notes:</strong><br/>{order.Notes}</p>")}

                        <div style='margin-top: 30px; padding: 15px; background: #f8f9fa; text-align: center; border-radius: 5px;'>
                            <p style='margin: 0 0 15px 0;'><strong>Browse Our Full Catalog</strong></p>
                            <a href='{_settings.CatalogueUrl}' style='display: inline-block; padding: 12px 30px; background: #a89f6e; color: white; text-decoration: none; border-radius: 4px; font-weight: bold;'>View Full Catalog</a>
                        </div>

                        <p style='margin-top: 30px; font-size: 13px; color: #777;'>
                            Our team will review your order and get back to you with pricing details shortly.<br/>
                            For any inquiries, please contact us at {(_settings.SenderEmail)}.
                        </p>
                    </div>
                </body>
            </html>";

            var payload = new
            {
                sender = new { name = _settings.SenderName, email = _settings.SenderEmail },
                to = new[] { new { name = customerName, email = order.CustomerEmail } },
                subject = $"Order Confirmation #{order.OrderId.ToString().PadLeft(5, '0')} - SA Funeral Supplies",
                htmlContent
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("smtp/email", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to send email via Brevo. Status: {status}, Error: {error}", response.StatusCode, error);
            }
            else
            {
                _logger.LogInformation("Order confirmation email sent successfully for Order #{orderId}", order.OrderId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while sending email for Order #{orderId}", order.OrderId);
        }
    }

    public async Task AddContactToMarketingListAsync(string email, string name, string phone)
    {
        try
        {
            if (_settings.MarketingListId <= 0) return;

            var payload = new
            {
                email = email,
                attributes = new { FIRSTNAME = name, SMS = phone },
                listIds = new[] { _settings.MarketingListId },
                updateEnabled = true
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("contacts", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to add contact to Brevo. Status: {status}, Error: {error}", response.StatusCode, error);
            }
            else
            {
                _logger.LogInformation("Contact {email} successfully added/updated in Brevo marketing list.", email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while adding contact {email} to marketing list", email);
        }
    }

    public async Task SendCatalogEmailAsync(string email, string name)
    {
        try
        {
            if (_settings.CatalogTemplateId <= 0) return;

            var payload = new
            {
                templateId = _settings.CatalogTemplateId,
                to = new[] { new { email = email, name = name } }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("smtp/email", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to send catalog email via Brevo. Status: {status}, Error: {error}", response.StatusCode, error);
            }
            else
            {
                _logger.LogInformation("Catalog email sent successfully to {email}", email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while sending catalog email to {email}", email);
        }
    }
}
