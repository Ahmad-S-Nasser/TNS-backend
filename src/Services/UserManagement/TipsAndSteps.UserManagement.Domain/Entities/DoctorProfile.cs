namespace TipsAndSteps.UserManagement.Domain.Entities;

public sealed class DoctorProfile
{
    public string  Id              { get; set; } = string.Empty; // same as User.Id
    public string  Specialty       { get; set; } = string.Empty;
    public string  LicenseNumber   { get; set; } = string.Empty;
    public string? HospitalName    { get; set; }
    public string? GovernorateCode { get; set; }
    public bool    IsApproved      { get; set; }
    public DateTime CreatedAt      { get; set; } = DateTime.UtcNow;
}
