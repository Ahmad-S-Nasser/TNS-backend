using MediatR;

namespace TipsAndSteps.UserManagement.Application.Queries.GetUser;

public sealed record GetUserQuery(string UserId) : IRequest<UserDto?>;

public sealed record UserDto(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Role,
    string? RoleCategory,
    string AccountStatus,
    List<PermissionOverrideDto> Overrides,
    string? GovernorateCode,
    string PreferredLanguage,
    bool IsActive,
    bool IsVerified,
    DateTime CreatedAt,
    DateTime? LastActive);

public sealed record PermissionOverrideDto(
    string Permission,
    string Action,
    string? Reason,
    string GrantedBy,
    DateTime Timestamp);
