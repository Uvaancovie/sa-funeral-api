namespace SAFuneralSuppliesAPI.DTOs;

public class LeadCaptureRequest
{
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
}