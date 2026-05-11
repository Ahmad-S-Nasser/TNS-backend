namespace TipsAndSteps.Shared.Infrastructure.Kafka;

public sealed class KafkaSettings
{
    public string BootstrapServers   { get; init; } = "localhost:6090";
    public string SchemaRegistryUrl  { get; init; } = "http://localhost:6091";
    public string GroupId            { get; init; } = string.Empty;
    public int    SessionTimeoutMs   { get; init; } = 30000;
    public string AutoOffsetReset    { get; init; } = "earliest";
}
