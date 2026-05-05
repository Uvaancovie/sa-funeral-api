namespace SAFuneralSuppliesAPI.Configuration;

public class BrevoSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public int MarketingListId { get; set; }
    public int CatalogTemplateId { get; set; }
    public string CatalogueUrl { get; set; } = string.Empty;
}
