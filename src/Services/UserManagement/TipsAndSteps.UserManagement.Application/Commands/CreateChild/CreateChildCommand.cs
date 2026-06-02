using MediatR;

namespace TipsAndSteps.UserManagement.Application.Commands.CreateChild;

public sealed record CreateChildCommand(
    string ParentId,
    string FullName,
    DateTime DateOfBirth,
    string Gender,
    string? BloodType) : IRequest<CreateChildResult>;

public sealed record CreateChildResult(
    string Id,
    string ParentId,
    string FullName);
