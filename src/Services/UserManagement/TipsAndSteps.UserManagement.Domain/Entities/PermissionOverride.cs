namespace TipsAndSteps.UserManagement.Domain.Entities;

public sealed class PermissionOverride
{
    public string Permission { get; set; } = string.Empty;
    public string Action { get; set; } = "grant"; // grant | deny
    public string? Reason { get; set; }
    public string GrantedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
