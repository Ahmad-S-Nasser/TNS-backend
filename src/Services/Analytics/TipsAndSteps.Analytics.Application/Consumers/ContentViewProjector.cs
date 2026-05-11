using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TipsAndSteps.Shared.Contracts;
using TipsAndSteps.Shared.Contracts.Events;
using TipsAndSteps.Shared.Infrastructure.Kafka;

namespace TipsAndSteps.Analytics.Application.Consumers;

/// <summary>
/// Projects ContentViewed events into the Analytics read-only MongoDB (tips-steps-analytics-read).
/// This is the CQRS analytics read model populated by Kafka.
/// </summary>
public sealed class ContentViewProjector : KafkaConsumerBase<ContentViewedEvent>
{
    public ContentViewProjector(
        IOptions<KafkaSettings> options,
        ILogger<ContentViewProjector> logger)
        : base(options, logger, KafkaTopics.ContentViewed, "tns-analytics-content-projector") { }

    protected override async Task HandleAsync(ContentViewedEvent message, CancellationToken ct)
    {
        Logger.LogInformation("Projecting content view: {ContentId} by {UserId}", message.ContentId, message.UserId);
        // TODO: Upsert ContentEngagementMetric in analytics read MongoDB
        await Task.CompletedTask;
    }
}
