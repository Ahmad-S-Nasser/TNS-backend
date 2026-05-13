using TipsAndSteps.UserManagement.Domain.Enums;

namespace TipsAndSteps.UserManagement.Domain.Entities;

/// <summary>
/// Represents an application user stored in MongoDB.
/// Authentication is handled via manual JWT (BCrypt password + HS256 token).
/// </summary>
public sealed class User
{
    public string    Id             { get; set; } = string.Empty;
    public string    Email          { get; set; } = string.Empty;
    public string    PasswordHash   { get; set; } = string.Empty;
    public string?   PhoneNumber    { get; set; }
    public string    FirstName      { get; set; } = string.Empty;
    public string    LastName       { get; set; } = string.Empty;
    public UserRole  Role           { get; set; }
    public RoleCategory? RoleCategory { get; set; }
    public string    AccountStatus  { get; set; } = "active"; // active | suspended | deactivated
    public List<PermissionOverride> Overrides { get; set; } = new();
    public string?   GovernorateCode { get; set; } // Egypt governorate
    public string    PreferredLanguage { get; set; } = "ar-EG";
    public bool      IsActive       { get; set; } = true;
    public bool      IsVerified     { get; set; }
    public DateTime  CreatedAt      { get; set; } = DateTime.UtcNow;
    public DateTime  UpdatedAt      { get; set; } = DateTime.UtcNow;
    public DateTime? LastActive      { get; set; }
    public string?   FcmToken       { get; set; } // Firebase Cloud Messaging
}
