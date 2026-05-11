using MediatR;
using TipsAndSteps.UserManagement.Application.Abstractions;
using TipsAndSteps.UserManagement.Application.Queries.GetUser;

namespace TipsAndSteps.UserManagement.Application.Queries.ListUsers;

public sealed class ListUsersHandler : IRequestHandler<ListUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IUserReadRepository _userRepository;

    public ListUsersHandler(IUserReadRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.ListAsync(request.Page, request.PageSize, cancellationToken);
        
        return users.Select(u => new UserDto(
            u.Id,
            u.Email,
            u.FirstName,
            u.LastName,
            u.PhoneNumber,
            u.Role.ToString(),
            u.RoleCategory?.ToString(),
            u.AccountStatus,
            u.Overrides.Select(o => new PermissionOverrideDto(
                o.Permission,
                o.Action,
                o.Reason,
                o.GrantedBy,
                o.Timestamp)).ToList(),
            u.GovernorateCode,
            u.PreferredLanguage,
            u.IsActive,
            u.IsVerified,
            u.CreatedAt,
            u.LastActive
        )).ToList();
    }
}
