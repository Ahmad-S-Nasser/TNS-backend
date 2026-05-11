namespace TipsAndSteps.UserManagement.Domain.Entities;

public sealed class ChildProfile
{
    public string   Id          { get; set; } = Guid.NewGuid().ToString();
    public string   ParentId    { get; set; } = string.Empty;
    public string   FullName    { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string   Gender      { get; set; } = string.Empty; // Male | Female
    public string?  BloodType   { get; set; }
    public bool     IsActive    { get; set; } = true;
    public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt   { get; set; } = DateTime.UtcNow;
}
