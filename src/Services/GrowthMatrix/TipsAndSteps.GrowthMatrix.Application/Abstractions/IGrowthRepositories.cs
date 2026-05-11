using TipsAndSteps.GrowthMatrix.Domain.Entities;

namespace TipsAndSteps.GrowthMatrix.Application.Abstractions;

public interface IGrowthSkillRepository
{
    Task<IReadOnlyList<GrowthSkill>> ListAsync(CancellationToken ct = default);
    Task<GrowthSkill?> FindByIdAsync(string id, CancellationToken ct = default);
    Task CreateAsync(GrowthSkill skill, CancellationToken ct = default);
    Task UpdateAsync(string id, GrowthSkill skill, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}

public interface IGrowthAssessmentRepository
{
    Task CreateAsync(GrowthAssessment assessment, CancellationToken ct = default);
    Task<GrowthAssessment?> FindByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<GrowthAssessment>> GetByChildIdAsync(string childId, CancellationToken ct = default);
    Task<IReadOnlyList<GrowthAssessment>> ListAsync(CancellationToken ct = default);
}

public interface IGrowthEventPublisher
{
    Task PublishAssessmentCompletedAsync(GrowthAssessment assessment, CancellationToken ct = default);
}

public interface IGrowthAgeGroupRepository
{
    Task<IReadOnlyList<GrowthAgeGroup>> ListAsync(CancellationToken ct = default);
    Task CreateAsync(GrowthAgeGroup ageGroup, CancellationToken ct = default);
    Task UpdateAsync(string id, GrowthAgeGroup ageGroup, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}

public interface IGrowthMatrixCategoryRepository
{
    Task<IReadOnlyList<GrowthMatrixCategory>> ListAsync(CancellationToken ct = default);
    Task CreateAsync(GrowthMatrixCategory category, CancellationToken ct = default);
    Task UpdateAsync(string id, GrowthMatrixCategory category, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}

public interface IGrowthRuleRepository
{
    Task<IReadOnlyList<GrowthRule>> ListAsync(CancellationToken ct = default);
    Task CreateAsync(GrowthRule rule, CancellationToken ct = default);
    Task UpdateAsync(string id, GrowthRule rule, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
