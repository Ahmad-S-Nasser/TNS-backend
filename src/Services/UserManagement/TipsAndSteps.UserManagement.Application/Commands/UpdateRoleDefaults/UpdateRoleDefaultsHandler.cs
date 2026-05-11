using MediatR;
using TipsAndSteps.UserManagement.Application.Abstractions;
using TipsAndSteps.UserManagement.Domain.Enums;

namespace TipsAndSteps.UserManagement.Application.Commands.UpdateRoleDefaults;

public sealed record UpdateRoleDefaultsCommand(string Category, List<string> Permissions) : IRequest<bool>;

public sealed class UpdateRoleDefaultsHandler : IRequestHandler<UpdateRoleDefaultsCommand, bool>
{
    private readonly IRoleDefaultRepository _repo;

    public UpdateRoleDefaultsHandler(IRoleDefaultRepository repo) => _repo = repo;

    public async Task<bool> Handle(UpdateRoleDefaultsCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<RoleCategory>(request.Category, true, out var category)) return false;

        await _repo.UpdateAsync(category, request.Permissions, ct);
        return true;
    }
}
