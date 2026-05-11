using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TipsAndSteps.Shared.Infrastructure;
using TipsAndSteps.Shared.Infrastructure.MongoDB;
using TipsAndSteps.UserManagement.Application.Abstractions;
using TipsAndSteps.UserManagement.Infrastructure.Kafka;
using TipsAndSteps.UserManagement.Infrastructure.Persistence;
using TipsAndSteps.UserManagement.Infrastructure.Persistence.Repositories;

namespace TipsAndSteps.UserManagement.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddUserManagementInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Shared (Kafka + Redis)
        services.AddSharedInfrastructure(configuration);

        // MongoDB
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDB"));
        services.AddSingleton<UserManagementDbContext>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserReadRepository, UserRepository>();
        services.AddScoped<IRoleDefaultRepository, RoleDefaultRepository>();

        // JWT Provider
        services.AddSingleton<IJwtProvider, JwtProvider>();

        // Kafka Publishers
        services.AddScoped<IUserEventPublisher, UserEventPublisher>();

        return services;
    }
}
