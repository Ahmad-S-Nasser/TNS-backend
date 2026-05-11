using MongoDB.Driver;
using TipsAndSteps.UserManagement.Application.Abstractions;
using TipsAndSteps.UserManagement.Domain.Entities;
using TipsAndSteps.UserManagement.Domain.Enums;
using TipsAndSteps.UserManagement.Infrastructure.Persistence;

namespace TipsAndSteps.UserManagement.Infrastructure.Persistence.Repositories;

public sealed class RoleDefaultRepository : IRoleDefaultRepository
{
    private readonly UserManagementDbContext _ctx;

    public RoleDefaultRepository(UserManagementDbContext ctx) => _ctx = ctx;

    public Task<List<RoleDefault>> GetAllAsync(CancellationToken ct = default)
        => _ctx.RoleDefaults.Find(_ => true).ToListAsync(ct);

    public async Task<RoleDefault?> GetByCategoryAsync(RoleCategory category, CancellationToken ct = default)
        => await _ctx.RoleDefaults.Find(d => d.Category == category).FirstOrDefaultAsync(ct);

    public async Task UpdateAsync(RoleCategory category, List<string> permissions, CancellationToken ct = default)
    {
        var filter = Builders<RoleDefault>.Filter.Eq(d => d.Category, category);
        var update = Builders<RoleDefault>.Update.Set(d => d.Permissions, permissions);
        await _ctx.RoleDefaults.UpdateOneAsync(filter, update, cancellationToken: ct);
    }
}
