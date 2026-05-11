using MediatR;
using TipsAndSteps.UserManagement.Domain.Enums;

namespace TipsAndSteps.UserManagement.Application.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    UserRole Role,
    string? GovernorateCode,
    string PreferredLanguage = "ar-EG"
) : IRequest<RegisterUserResult>;

public sealed record RegisterUserResult(
    string UserId,
    string KeycloakId,
    string Email,
    string Role);
