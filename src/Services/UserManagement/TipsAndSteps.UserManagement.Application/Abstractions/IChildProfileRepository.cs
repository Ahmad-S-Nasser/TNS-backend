using TipsAndSteps.UserManagement.Domain.Entities;

namespace TipsAndSteps.UserManagement.Application.Abstractions;

/// <summary>Write repository for child profiles — targets MongoDB primary.</summary>
public interface IChildProfileRepository
{
    Task CreateAsync(ChildProfile child, CancellationToken ct = default);
    Task UpdateAsync(ChildProfile child, CancellationToken ct = default);
    Task<ChildProfile?> FindByIdAsync(string id, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}

/// <summary>Read repository for child profiles — targets MongoDB read replica.</summary>
public interface IChildProfileReadRepository
{
    Task<ChildProfile?> FindByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<ChildProfile>> FindByParentIdAsync(string parentId, CancellationToken ct = default);
}
