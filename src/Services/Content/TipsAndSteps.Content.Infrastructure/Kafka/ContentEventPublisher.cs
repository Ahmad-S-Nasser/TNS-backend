using TipsAndSteps.Content.Application.Abstractions;
using TipsAndSteps.Content.Domain.Entities;
using TipsAndSteps.Shared.Contracts;
using TipsAndSteps.Shared.Contracts.Events;
using TipsAndSteps.Shared.Infrastructure.Kafka;

namespace TipsAndSteps.Content.Infrastructure.Kafka;

public sealed class ContentEventPublisher : IContentEventPublisher
{
    private readonly IKafkaProducerService _producer;
    public ContentEventPublisher(IKafkaProducerService producer) => _producer = producer;

    public Task PublishContentPublishedAsync(ContentArticle article, CancellationToken ct = default)
        => _producer.PublishAsync(KafkaTopics.ContentPublished, new
        {
            ContentId   = article.Id,
            Section     = article.Section.ToString(),
            TitleAr     = article.TitleAr,
            TitleEn     = article.TitleEn,
            PublishedAt = article.PublishedAt
        }, ct);

    public Task PublishContentViewedAsync(
        string contentId, string contentType, string section, string userId, CancellationToken ct = default)
        => _producer.PublishAsync(KafkaTopics.ContentViewed, new ContentViewedEvent(
            contentId, contentType, section, userId, null, DateTime.UtcNow), ct);
}
