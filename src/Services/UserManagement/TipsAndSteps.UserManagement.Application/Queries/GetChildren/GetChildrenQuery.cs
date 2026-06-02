using MediatR;

namespace TipsAndSteps.UserManagement.Application.Queries.GetChildren;

public sealed record GetChildrenQuery(string ParentId) : IRequest<IReadOnlyList<ChildDto>>;

public sealed record ChildDto(
    string Id,
    string ParentId,
    string FullName,
    DateTime DateOfBirth,
    string Gender,
    string? BloodType,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
