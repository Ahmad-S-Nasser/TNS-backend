using MediatR;

namespace TipsAndSteps.UserManagement.Application.Commands.UpdateProfile;

/// <summary>
/// Update the current user's own profile fields (all fields are optional — only non-null values are applied).
/// </summary>
public sealed record UpdateProfileCommand(
    string UserId,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? PreferredLanguage) : IRequest<bool>;
