namespace TipsAndSteps.Shared.Infrastructure.MongoDB;

public sealed class MongoDbSettings
{
    public string ConnectionString { get; init; } = string.Empty;
    public string DatabaseName     { get; init; } = string.Empty;
    public string ReadReplicaConnectionString { get; init; } = string.Empty;
}
