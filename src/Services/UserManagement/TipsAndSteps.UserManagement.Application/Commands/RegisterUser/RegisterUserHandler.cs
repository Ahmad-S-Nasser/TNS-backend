using MediatR;
using BCryptNet = BCrypt.Net.BCrypt;
using TipsAndSteps.UserManagement.Application.Abstractions;
using TipsAndSteps.UserManagement.Domain.Entities;

namespace TipsAndSteps.UserManagement.Application.Commands.RegisterUser;

public sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUserRepository      _userRepo;
    private readonly IUserEventPublisher  _eventPublisher;

    public RegisterUserHandler(
        IUserRepository userRepo,
        IUserEventPublisher eventPublisher)
    {
        _userRepo       = userRepo;
        _eventPublisher = eventPublisher;
    }

    public async Task<RegisterUserResult> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Hash password
        var passwordHash = BCryptNet.HashPassword(request.Password);
        var userId = Guid.NewGuid().ToString();

        // 2. Create user in MongoDB
        var user = new User
        {
            Id              = userId,
            Email           = request.Email,
            PasswordHash    = passwordHash,
            FirstName       = request.FirstName,
            LastName        = request.LastName,
            PhoneNumber     = request.PhoneNumber,
            Role            = request.Role,
            GovernorateCode = request.GovernorateCode,
            PreferredLanguage = request.PreferredLanguage,
            CreatedAt       = DateTime.UtcNow,
            UpdatedAt       = DateTime.UtcNow
        };
        await _userRepo.CreateAsync(user, cancellationToken);

        // 3. Publish Kafka event for downstream services
        await _eventPublisher.PublishUserRegisteredAsync(user, cancellationToken);

        return new RegisterUserResult(
            user.Id,
            user.Id, // Mirroring for compatibility
            user.Email,
            user.Role.ToString());
    }
}
