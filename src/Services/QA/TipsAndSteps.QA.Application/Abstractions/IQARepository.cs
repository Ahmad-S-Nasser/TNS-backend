using TipsAndSteps.QA.Domain.Entities;

namespace TipsAndSteps.QA.Application.Abstractions;

public interface IQARepository
{
    Task<Question?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<Question>> GetAllAsync(CancellationToken ct = default);
    Task CreateAsync(Question question, CancellationToken ct = default);
    Task UpdateAsync(Question question, CancellationToken ct = default);
}
