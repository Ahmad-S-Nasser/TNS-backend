using MediatR;
using TipsAndSteps.UserManagement.Application.Abstractions;

namespace TipsAndSteps.UserManagement.Application.Queries.GetChildren;

public sealed class GetChildrenHandler : IRequestHandler<GetChildrenQuery, IReadOnlyList<ChildDto>>
{
    private readonly IChildProfileReadRepository _readRepo;

    public GetChildrenHandler(IChildProfileReadRepository readRepo)
        => _readRepo = readRepo;

    public async Task<IReadOnlyList<ChildDto>> Handle(GetChildrenQuery request, CancellationToken cancellationToken)
    {
        var children = await _readRepo.FindByParentIdAsync(request.ParentId, cancellationToken);

        return children.Select(c => new ChildDto(
            c.Id,
            c.ParentId,
            c.FullName,
            c.DateOfBirth,
            c.Gender,
            c.BloodType,
            c.IsActive,
            c.CreatedAt,
            c.UpdatedAt)).ToList();
    }
}
