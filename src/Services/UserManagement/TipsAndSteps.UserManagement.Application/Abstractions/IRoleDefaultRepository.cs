using TipsAndSteps.UserManagement.Domain.Entities;
using TipsAndSteps.UserManagement.Domain.Enums;

namespace TipsAndSteps.UserManagement.Application.Abstractions;

public interface IRoleDefaultRepository
{
    Task<List<RoleDefault>> GetAllAsync(CancellationToken ct = default);
    Task<RoleDefault?> GetByCategoryAsync(RoleCategory category, CancellationToken ct = default);
    Task UpdateAsync(RoleCategory category, List<string> permissions, CancellationToken ct = default);
}
