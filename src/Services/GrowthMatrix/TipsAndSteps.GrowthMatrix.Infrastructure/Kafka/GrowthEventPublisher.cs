using TipsAndSteps.GrowthMatrix.Application.Abstractions;
using TipsAndSteps.GrowthMatrix.Domain.Entities;
using TipsAndSteps.Shared.Contracts;
using TipsAndSteps.Shared.Contracts.Events;
using TipsAndSteps.Shared.Infrastructure.Kafka;

namespace TipsAndSteps.GrowthMatrix.Infrastructure.Kafka;

public sealed class GrowthEventPublisher : IGrowthEventPublisher
{
    private readonly IKafkaProducerService _producer;
    public GrowthEventPublisher(IKafkaProducerService producer) => _producer = producer;

    public Task PublishAssessmentCompletedAsync(GrowthAssessment assessment, CancellationToken ct = default)
        => _producer.PublishAsync(KafkaTopics.GrowthAssessmentCompleted, new GrowthAssessmentCompletedEvent(
            assessment.Id,
            assessment.ChildId,
            assessment.ParentId,
            assessment.AgeGroupId,
            string.Join(",", assessment.CategoryScores.Keys),
            assessment.TotalScore,
            assessment.ScoreLevel.ToString(),
            assessment.CompletedAt), ct);

    public Task PublishGrowthAlertAsync(string childId, string message, CancellationToken ct = default)
        => _producer.PublishAsync(KafkaTopics.GrowthAlertTriggered,
            new { ChildId = childId, Message = message, AlertedAt = DateTime.UtcNow }, ct);
}
