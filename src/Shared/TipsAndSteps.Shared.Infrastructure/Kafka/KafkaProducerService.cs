using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace TipsAndSteps.Shared.Infrastructure.Kafka;

public interface IKafkaProducerService
{
    Task PublishAsync<T>(string topic, T message, CancellationToken ct = default) where T : class;
}

public sealed class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;

    public KafkaProducerService(
        IOptions<KafkaSettings> options,
        ILogger<KafkaProducerService> logger)
    {
        _logger = logger;
        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            Acks = Acks.All,           // acks=all for durability
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000,
            EnableIdempotence = true,
            MessageTimeoutMs = 5000,   // 5 seconds max
            SocketTimeoutMs = 5000     // 5 seconds max
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, T message, CancellationToken ct = default) where T : class
    {
        var key     = Guid.NewGuid().ToString();
        var payload = JsonSerializer.Serialize(message);

        try
        {
            var result = await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key   = key,
                Value = payload
            }, ct);

            _logger.LogInformation(
                "Published to {Topic} [partition {Partition}] offset {Offset}",
                result.Topic, result.Partition, result.Offset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to publish to {Topic}: {Error}", topic, ex.Error.Reason);
            throw;
        }
    }

    public void Dispose() => _producer.Dispose();
}
