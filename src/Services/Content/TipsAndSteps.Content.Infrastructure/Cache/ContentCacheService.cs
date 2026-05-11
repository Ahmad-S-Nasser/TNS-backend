using TipsAndSteps.Content.Application.Abstractions;
using TipsAndSteps.Shared.Infrastructure.Redis;

namespace TipsAndSteps.Content.Infrastructure.Cache;

public sealed class ContentCacheService : IContentCacheService
{
    private readonly IRedisCacheService _redis;
    public ContentCacheService(IRedisCacheService redis) => _redis = redis;

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
        => _redis.GetAsync<T>(key, ct);

    public Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default) where T : class
        => _redis.SetAsync(key, value, ttl, ct);

    public Task RemoveAsync(string key, CancellationToken ct = default)
        => _redis.RemoveAsync(key, ct);
}
