using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace TipsAndSteps.Shared.Infrastructure.Redis;

public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default) where T : class;
    Task RemoveAsync(string key, CancellationToken ct = default);
}

public sealed class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _db;
    private readonly TimeSpan _defaultTtl;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(
        IConnectionMultiplexer redis,
        IOptions<RedisSettings> options,
        ILogger<RedisCacheService> logger)
    {
        _db         = redis.GetDatabase();
        _defaultTtl = TimeSpan.FromMinutes(options.Value.DefaultTtlMinutes);
        _logger     = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        var raw = await _db.StringGetAsync(key);
        if (!raw.HasValue) return null;
        return JsonSerializer.Deserialize<T>(raw!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default) where T : class
    {
        var payload = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, payload, ttl ?? _defaultTtl);
        _logger.LogDebug("Cache SET {Key} ttl={Ttl}", key, ttl ?? _defaultTtl);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
        => await _db.KeyDeleteAsync(key);
}
