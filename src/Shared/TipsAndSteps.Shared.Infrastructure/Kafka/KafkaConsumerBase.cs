using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace TipsAndSteps.Shared.Infrastructure.Kafka;

/// <summary>
/// Generic background consumer. Derive and implement HandleAsync for each consumer.
/// Uses manual commit (enable.auto.commit=false) for exactly-once semantics.
/// </summary>
public abstract class KafkaConsumerBase<TMessage> : BackgroundService where TMessage : class
{
    private readonly KafkaSettings _settings;
    protected readonly ILogger Logger;
    private readonly string _topic;
    private readonly string _groupId;

    protected KafkaConsumerBase(
        IOptions<KafkaSettings> options,
        ILogger logger,
        string topic,
        string? groupId = null)
    {
        _settings = options.Value;
        Logger    = logger;
        _topic    = topic;
        _groupId  = groupId ?? _settings.GroupId;
    }

    protected abstract Task HandleAsync(TMessage message, CancellationToken ct);

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => ConsumeLoop(stoppingToken), stoppingToken);
    }

    private void ConsumeLoop(CancellationToken ct)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers      = _settings.BootstrapServers,
            GroupId               = _groupId,
            AutoOffsetReset       = AutoOffsetReset.Earliest,
            EnableAutoCommit      = false,    // manual commit
            SessionTimeoutMs      = _settings.SessionTimeoutMs
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_topic);
        Logger.LogInformation("Kafka consumer subscribed to {Topic}", _topic);

        try
        {
            while (!ct.IsCancellationRequested)
            {
                var result = consumer.Consume(ct);
                if (result is null) continue;

                try
                {
                    var msg = JsonSerializer.Deserialize<TMessage>(result.Message.Value)
                              ?? throw new InvalidOperationException("Null deserialization result");

                    HandleAsync(msg, ct).GetAwaiter().GetResult();
                    consumer.Commit(result);  // manual commit after successful processing
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error processing Kafka message from {Topic}. "
                        + "Message will NOT be committed (will be retried).", _topic);
                }
            }
        }
        catch (OperationCanceledException) { /* graceful shutdown */ }
        finally
        {
            consumer.Close();
        }
    }
}
