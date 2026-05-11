using TipsAndSteps.UserManagement.Domain.Enums;

namespace TipsAndSteps.UserManagement.Domain.Entities;

public sealed class RoleDefault
{
    public string Id { get; set; } = string.Empty;
    public RoleCategory Category { get; set; }
    public List<string> Permissions { get; set; } = new();
}
