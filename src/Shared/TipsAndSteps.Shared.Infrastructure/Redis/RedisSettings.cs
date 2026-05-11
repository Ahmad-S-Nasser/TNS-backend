namespace TipsAndSteps.Shared.Infrastructure.Redis;

public sealed class RedisSettings
{
    public string ConnectionString { get; init; } = "localhost:6379";
    public int    DefaultTtlMinutes { get; init; } = 60;
}
