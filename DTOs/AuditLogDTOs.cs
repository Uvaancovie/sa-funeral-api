namespace SAFuneralSuppliesAPI.DTOs;

public class AuditLogResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public bool Success { get; set; }
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; }
}

public class AuditLogsListResponse
{
    public List<AuditLogResponse> Logs { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
