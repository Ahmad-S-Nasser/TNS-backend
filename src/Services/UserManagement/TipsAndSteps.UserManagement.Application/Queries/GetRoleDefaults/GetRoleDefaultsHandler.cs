using MediatR;
using TipsAndSteps.UserManagement.Application.Abstractions;

namespace TipsAndSteps.UserManagement.Application.Queries.GetRoleDefaults;

public sealed record GetRoleDefaultsQuery() : IRequest<List<RoleDefaultDto>>;

public sealed record RoleDefaultDto(string Category, List<string> Permissions);

public sealed class GetRoleDefaultsHandler : IRequestHandler<GetRoleDefaultsQuery, List<RoleDefaultDto>>
{
    private readonly IRoleDefaultRepository _repo;

    public GetRoleDefaultsHandler(IRoleDefaultRepository repo) => _repo = repo;

    public async Task<List<RoleDefaultDto>> Handle(GetRoleDefaultsQuery request, CancellationToken ct)
    {
        var defaults = await _repo.GetAllAsync(ct);
        return defaults.Select(d => new RoleDefaultDto(d.Category.ToString(), d.Permissions)).ToList();
    }
}
