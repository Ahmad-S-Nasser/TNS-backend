using MediatR;
using TipsAndSteps.UserManagement.Application.Abstractions;

namespace TipsAndSteps.UserManagement.Application.Queries.GetUser;

public sealed class GetUserHandler : IRequestHandler<GetUserQuery, UserDto?>
{
    private readonly IUserReadRepository _readRepo;

    public GetUserHandler(IUserReadRepository readRepo)
        => _readRepo = readRepo;

    public async Task<UserDto?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _readRepo.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return null;

        return new UserDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.Role.ToString(),
            user.RoleCategory?.ToString(),
            user.AccountStatus,
            user.Overrides.Select(o => new PermissionOverrideDto(
                o.Permission,
                o.Action,
                o.Reason,
                o.GrantedBy,
                o.Timestamp)).ToList(),
            user.GovernorateCode,
            user.PreferredLanguage,
            user.IsActive,
            user.IsVerified,
            user.CreatedAt,
            user.LastActive);
    }
}
