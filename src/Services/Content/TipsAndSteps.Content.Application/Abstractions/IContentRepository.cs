using TipsAndSteps.Content.Domain.Entities;
using TipsAndSteps.Content.Domain.Enums;

namespace TipsAndSteps.Content.Application.Abstractions;

public interface IContentRepository
{
    Task CreateAsync(ContentArticle article, CancellationToken ct = default);
    Task UpdateAsync(ContentArticle article, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
    Task<ContentArticle?> FindByIdAsync(string id, CancellationToken ct = default);
}

public interface IContentReadRepository
{
    Task<ContentArticle?> FindByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<ContentArticle>> ListBySectionAsync(
        ContentSection section, int page, int pageSize, ContentStatus? status = null, CancellationToken ct = default);
    Task<IReadOnlyList<ContentArticle>> SearchAsync(
        string query, string language, int page, int pageSize, CancellationToken ct = default);
    Task<IReadOnlyList<ContentArticle>> ListAllAsync(CancellationToken ct = default);
    Task<Dictionary<string, int>> GetStatsAsync(CancellationToken ct = default);
}

public interface IContentCacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default) where T : class;
    Task RemoveAsync(string key, CancellationToken ct = default);
}

public interface IContentEventPublisher
{
    Task PublishContentPublishedAsync(ContentArticle article, CancellationToken ct = default);
    Task PublishContentViewedAsync(string contentId, string contentType, string section, string userId, CancellationToken ct = default);
}
