using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TipsAndSteps.GrowthMatrix.Application.Abstractions;
using TipsAndSteps.GrowthMatrix.Domain.Entities;
using TipsAndSteps.Shared.Infrastructure.MongoDB;

namespace TipsAndSteps.GrowthMatrix.Infrastructure.Persistence.Repositories;

public sealed class GrowthDbContext : MongoDbContext
{
    public IMongoCollection<GrowthSkill>      Skills      => WriteCollection<GrowthSkill>("growth_skills");
    public IMongoCollection<GrowthAssessment> Assessments => WriteCollection<GrowthAssessment>("assessments");
    public IMongoCollection<GrowthAgeGroup>   AgeGroups   => WriteCollection<GrowthAgeGroup>("growth_age_groups");
    public IMongoCollection<GrowthMatrixCategory> Categories => WriteCollection<GrowthMatrixCategory>("growth_categories");
    public IMongoCollection<GrowthRule>       Rules       => WriteCollection<GrowthRule>("growth_rules");

    public GrowthDbContext(IOptions<MongoDbSettings> options) : base(options) 
    { 
        RegisterMap<GrowthSkill>();
        RegisterMap<GrowthAssessment>();
        RegisterMap<GrowthAgeGroup>();
        RegisterMap<GrowthMatrixCategory>();
        RegisterMap<GrowthRule>();
    }

    private void RegisterMap<T>() where T : class
    {
        if (!MongoDB.Bson.Serialization.BsonClassMap.IsClassMapRegistered(typeof(T)))
        {
            MongoDB.Bson.Serialization.BsonClassMap.RegisterClassMap<T>(cm => {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                var idMember = cm.GetMemberMap("Id");
                if (idMember != null)
                {
                    idMember.SetIdGenerator(MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator.Instance)
                             .SetSerializer(new MongoDB.Bson.Serialization.Serializers.StringSerializer(MongoDB.Bson.BsonType.ObjectId));
                }
            });
        }
    }
}

public sealed class GrowthAgeGroupRepository : IGrowthAgeGroupRepository
{
    private readonly GrowthDbContext _ctx;
    public GrowthAgeGroupRepository(GrowthDbContext ctx) => _ctx = ctx;

    public async Task<IReadOnlyList<GrowthAgeGroup>> ListAsync(CancellationToken ct = default)
        => await _ctx.AgeGroups.Find(_ => true).ToListAsync(ct);

    public Task CreateAsync(GrowthAgeGroup ageGroup, CancellationToken ct = default)
        => _ctx.AgeGroups.InsertOneAsync(ageGroup, cancellationToken: ct);

    public Task UpdateAsync(string id, GrowthAgeGroup ageGroup, CancellationToken ct = default)
        => _ctx.AgeGroups.ReplaceOneAsync(ag => ag.Id == id, ageGroup, cancellationToken: ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _ctx.AgeGroups.DeleteOneAsync(ag => ag.Id == id, cancellationToken: ct);
}

public sealed class GrowthMatrixCategoryRepository : IGrowthMatrixCategoryRepository
{
    private readonly GrowthDbContext _ctx;
    public GrowthMatrixCategoryRepository(GrowthDbContext ctx) => _ctx = ctx;

    public async Task<IReadOnlyList<GrowthMatrixCategory>> ListAsync(CancellationToken ct = default)
        => await _ctx.Categories.Find(_ => true).SortBy(c => c.SortOrder).ToListAsync(ct);

    public Task CreateAsync(GrowthMatrixCategory category, CancellationToken ct = default)
        => _ctx.Categories.InsertOneAsync(category, cancellationToken: ct);

    public Task UpdateAsync(string id, GrowthMatrixCategory category, CancellationToken ct = default)
        => _ctx.Categories.ReplaceOneAsync(c => c.Id == id, category, cancellationToken: ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _ctx.Categories.DeleteOneAsync(c => c.Id == id, cancellationToken: ct);
}

public sealed class GrowthRuleRepository : IGrowthRuleRepository
{
    private readonly GrowthDbContext _ctx;
    public GrowthRuleRepository(GrowthDbContext ctx) => _ctx = ctx;

    public async Task<IReadOnlyList<GrowthRule>> ListAsync(CancellationToken ct = default)
        => await _ctx.Rules.Find(_ => true).ToListAsync(ct);

    public Task CreateAsync(GrowthRule rule, CancellationToken ct = default)
        => _ctx.Rules.InsertOneAsync(rule, cancellationToken: ct);

    public Task UpdateAsync(string id, GrowthRule rule, CancellationToken ct = default)
        => _ctx.Rules.ReplaceOneAsync(r => r.Id == id, rule, cancellationToken: ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _ctx.Rules.DeleteOneAsync(r => r.Id == id, cancellationToken: ct);
}

public sealed class GrowthSkillRepository : IGrowthSkillRepository
{
    private readonly GrowthDbContext _ctx;
    public GrowthSkillRepository(GrowthDbContext ctx) => _ctx = ctx;

    public async Task<IReadOnlyList<GrowthSkill>> ListAsync(CancellationToken ct = default)
        => await _ctx.Skills.Find(_ => true).SortBy(s => s.SortOrder).ToListAsync(ct);

    public async Task<GrowthSkill?> FindByIdAsync(string id, CancellationToken ct = default)
        => await _ctx.Skills.Find(s => s.Id == id).FirstOrDefaultAsync(ct);

    public Task CreateAsync(GrowthSkill skill, CancellationToken ct = default)
        => _ctx.Skills.InsertOneAsync(skill, cancellationToken: ct);

    public Task UpdateAsync(string id, GrowthSkill skill, CancellationToken ct = default)
        => _ctx.Skills.ReplaceOneAsync(s => s.Id == id, skill, cancellationToken: ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _ctx.Skills.DeleteOneAsync(s => s.Id == id, cancellationToken: ct);
}

public sealed class GrowthAssessmentRepository : IGrowthAssessmentRepository
{
    private readonly GrowthDbContext _ctx;
    public GrowthAssessmentRepository(GrowthDbContext ctx) => _ctx = ctx;

    public Task CreateAsync(GrowthAssessment assessment, CancellationToken ct = default)
        => _ctx.Assessments.InsertOneAsync(assessment, cancellationToken: ct);

    public async Task<GrowthAssessment?> FindByIdAsync(string id, CancellationToken ct = default)
        => await _ctx.Assessments.Find(a => a.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<GrowthAssessment>> GetByChildIdAsync(string childId, CancellationToken ct = default)
        => await _ctx.Assessments
                     .Find(a => a.ChildId == childId)
                     .SortByDescending(a => a.CompletedAt)
                     .ToListAsync(ct);

    public async Task<IReadOnlyList<GrowthAssessment>> ListAsync(CancellationToken ct = default)
        => await _ctx.Assessments.Find(_ => true).ToListAsync(ct);
}
