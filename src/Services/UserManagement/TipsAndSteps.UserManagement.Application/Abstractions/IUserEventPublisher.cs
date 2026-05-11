using TipsAndSteps.UserManagement.Domain.Entities;

namespace TipsAndSteps.UserManagement.Application.Abstractions;

public interface IUserEventPublisher
{
    Task PublishUserRegisteredAsync(User user, CancellationToken ct = default);
    Task PublishChildProfileCreatedAsync(ChildProfile child, CancellationToken ct = default);
}
