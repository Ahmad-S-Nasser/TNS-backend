using MediatR;
using TipsAndSteps.UserManagement.Application.Abstractions;
using TipsAndSteps.UserManagement.Domain.Entities;

namespace TipsAndSteps.UserManagement.Application.Commands.UpdateUserPermissions;

public sealed record UpdateUserPermissionsCommand(
    string UserId, 
    List<PermissionOverrideDto> Overrides) : IRequest<bool>;

public sealed record PermissionOverrideDto(
    string Permission,
    string Action,
    string? Reason);

public sealed class UpdateUserPermissionsHandler : IRequestHandler<UpdateUserPermissionsCommand, bool>
{
    private readonly IUserRepository _repo;

    public UpdateUserPermissionsHandler(IUserRepository repo) => _repo = repo;

    public async Task<bool> Handle(UpdateUserPermissionsCommand request, CancellationToken ct)
    {
        var user = await _repo.FindByIdAsync(request.UserId, ct);
        if (user == null) return false;

        user.Overrides = request.Overrides.Select(o => new PermissionOverride
        {
            Permission = o.Permission,
            Action = o.Action,
            Reason = o.Reason,
            Timestamp = DateTime.UtcNow,
            GrantedBy = "system-admin" // Should be from context
        }).ToList();

        await _repo.UpdateAsync(user, ct);
        return true;
    }
}
