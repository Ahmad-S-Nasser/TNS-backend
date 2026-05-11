using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TipsAndSteps.Shared.Contracts;
using TipsAndSteps.Shared.Contracts.Events;
using TipsAndSteps.Shared.Infrastructure.Kafka;

namespace TipsAndSteps.Notification.Application.Consumers;

/// <summary>
/// Consumes NotificationRequested events from Kafka and dispatches via FCM.
/// </summary>
public sealed class NotificationRequestConsumer : KafkaConsumerBase<NotificationRequestedEvent>
{
    public NotificationRequestConsumer(
        IOptions<KafkaSettings> options,
        ILogger<NotificationRequestConsumer> logger)
        : base(options, logger, KafkaTopics.NotificationRequested) { }

    protected override async Task HandleAsync(NotificationRequestedEvent message, CancellationToken ct)
    {
        Logger.LogInformation(
            "Sending notification {Id} to user {UserId}: {Title}",
            message.NotificationId, message.UserId, message.Title);

        // TODO: Inject IFcmService and call SendAsync(message.UserId, message.Title, message.Body, message.Data)
        await Task.CompletedTask;
    }
}
