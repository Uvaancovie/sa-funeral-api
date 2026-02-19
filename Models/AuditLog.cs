using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAFuneralSuppliesAPI.Models;

[Table("AuditLogs")]
public class AuditLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? CompanyName { get; set; }

    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty; // login, logout, login_failed

    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty; // customer, admin

    [MaxLength(50)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    public bool Success { get; set; } = true;

    [MaxLength(500)]
    public string? Details { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
