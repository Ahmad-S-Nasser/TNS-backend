using MongoDB.Driver;
using TipsAndSteps.UserManagement.Application.Abstractions;
using TipsAndSteps.UserManagement.Domain.Entities;
using TipsAndSteps.UserManagement.Infrastructure.Persistence;

namespace TipsAndSteps.UserManagement.Infrastructure.Persistence.Repositories;

public sealed class ChildProfileRepository : IChildProfileRepository, IChildProfileReadRepository
{
    private readonly UserManagementDbContext _ctx;

    public ChildProfileRepository(UserManagementDbContext ctx) => _ctx = ctx;

    // Write operations
    public async Task CreateAsync(ChildProfile child, CancellationToken ct = default)
        => await _ctx.Children.InsertOneAsync(child, cancellationToken: ct);

    public async Task UpdateAsync(ChildProfile child, CancellationToken ct = default)
    {
        var filter = Builders<ChildProfile>.Filter.Eq(c => c.Id, child.Id);
        child.UpdatedAt = DateTime.UtcNow;
        await _ctx.Children.ReplaceOneAsync(filter, child, cancellationToken: ct);
    }

    public async Task<ChildProfile?> FindByIdAsync(string id, CancellationToken ct = default)
        => await _ctx.Children.Find(c => c.Id == id).FirstOrDefaultAsync(ct);

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        var filter = Builders<ChildProfile>.Filter.Eq(c => c.Id, id);
        await _ctx.Children.DeleteOneAsync(filter, cancellationToken: ct);
    }

    // Read operations
    async Task<ChildProfile?> IChildProfileReadRepository.FindByIdAsync(string id, CancellationToken ct)
        => await _ctx.ChildrenRead.Find(c => c.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<ChildProfile>> FindByParentIdAsync(string parentId, CancellationToken ct = default)
    {
        return await _ctx.ChildrenRead
            .Find(c => c.ParentId == parentId && c.IsActive)
            .ToListAsync(ct);
    }
}
