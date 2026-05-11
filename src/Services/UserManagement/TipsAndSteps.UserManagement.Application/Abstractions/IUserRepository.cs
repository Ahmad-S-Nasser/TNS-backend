using TipsAndSteps.UserManagement.Domain.Entities;

namespace TipsAndSteps.UserManagement.Application.Abstractions;

/// <summary>Write repository — targets MongoDB primary.</summary>
public interface IUserRepository
{
    Task CreateAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
    Task<User?> FindByIdAsync(string id, CancellationToken ct = default);
}

/// <summary>Read repository — targets MongoDB read replica.</summary>
public interface IUserReadRepository
{
    Task<User?> FindByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<User>> ListAsync(int page, int pageSize, CancellationToken ct = default);
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);
}
