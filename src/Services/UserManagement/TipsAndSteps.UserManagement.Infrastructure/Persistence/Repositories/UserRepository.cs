using MongoDB.Driver;
using TipsAndSteps.UserManagement.Application.Abstractions;
using TipsAndSteps.UserManagement.Domain.Entities;
using TipsAndSteps.UserManagement.Infrastructure.Persistence;

namespace TipsAndSteps.UserManagement.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository, IUserReadRepository
{
    private readonly UserManagementDbContext _ctx;

    public UserRepository(UserManagementDbContext ctx) => _ctx = ctx;

    // Write operations → primary
    public async Task CreateAsync(User user, CancellationToken ct = default)
        => await _ctx.Users.InsertOneAsync(user, cancellationToken: ct);

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
        user.UpdatedAt = DateTime.UtcNow;
        await _ctx.Users.ReplaceOneAsync(filter, user, cancellationToken: ct);
    }

    public async Task<User?> FindByIdAsync(string id, CancellationToken ct = default)
        => await _ctx.Users.Find(u => u.Id == id).FirstOrDefaultAsync(ct);

    // Read operations → replica
    public async Task<IReadOnlyList<User>> ListAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var result = await _ctx.UsersRead
            .Find(_ => true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);
        return result;
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken ct = default)
        => await _ctx.UsersRead.Find(u => u.Email == email).FirstOrDefaultAsync(ct);
}
