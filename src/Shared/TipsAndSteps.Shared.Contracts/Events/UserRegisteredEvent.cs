namespace TipsAndSteps.Shared.Contracts.Events;

public sealed record UserRegisteredEvent(
    string UserId,
    string Email,
    string Role,           // parent | doctor | admin | superadmin
    string? PhoneNumber,
    DateTime RegisteredAt);
