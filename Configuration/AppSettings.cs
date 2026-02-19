namespace SAFuneralSuppliesAPI.Configuration;

public class ConnectionStringsSettings
{
    public string DefaultConnection { get; set; } = string.Empty;
}

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationDays { get; set; } = 7;
}
