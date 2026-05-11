using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TipsAndSteps.Shared.Infrastructure.Kafka;
using TipsAndSteps.Shared.Infrastructure.Redis;

namespace TipsAndSteps.Shared.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Kafka
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

        // Redis
        services.Configure<RedisSettings>(configuration.GetSection("Redis"));
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(
                configuration["Redis:ConnectionString"] ?? "localhost:6379"));
        services.AddSingleton<IRedisCacheService, RedisCacheService>();

        return services;
    }
}
