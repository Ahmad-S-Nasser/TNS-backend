using TipsAndSteps.Shared.Contracts;
using TipsAndSteps.Shared.Contracts.Events;
using TipsAndSteps.Shared.Infrastructure.Kafka;
using TipsAndSteps.UserManagement.Application.Abstractions;
using TipsAndSteps.UserManagement.Domain.Entities;

namespace TipsAndSteps.UserManagement.Infrastructure.Kafka;

public sealed class UserEventPublisher : IUserEventPublisher
{
    private readonly IKafkaProducerService _producer;

    public UserEventPublisher(IKafkaProducerService producer)
        => _producer = producer;

    public Task PublishUserRegisteredAsync(User user, CancellationToken ct = default)
        => _producer.PublishAsync(KafkaTopics.UserRegistered, new UserRegisteredEvent(
            user.Id,
            user.Email,
            user.Role.ToString().ToLower(),
            user.PhoneNumber,
            user.CreatedAt), ct);

    public Task PublishChildProfileCreatedAsync(ChildProfile child, CancellationToken ct = default)
        => _producer.PublishAsync(KafkaTopics.ChildProfileCreated, new ChildProfileCreatedEvent(
            child.Id,
            child.ParentId,
            child.FullName,
            child.DateOfBirth,
            child.Gender,
            child.CreatedAt), ct);
}
