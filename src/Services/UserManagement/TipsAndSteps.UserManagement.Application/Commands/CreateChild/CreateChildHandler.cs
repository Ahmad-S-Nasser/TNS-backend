using MediatR;
using TipsAndSteps.UserManagement.Application.Abstractions;
using TipsAndSteps.UserManagement.Domain.Entities;

namespace TipsAndSteps.UserManagement.Application.Commands.CreateChild;

public sealed class CreateChildHandler : IRequestHandler<CreateChildCommand, CreateChildResult>
{
    private readonly IChildProfileRepository _repository;

    public CreateChildHandler(IChildProfileRepository repository)
        => _repository = repository;

    public async Task<CreateChildResult> Handle(CreateChildCommand request, CancellationToken cancellationToken)
    {
        var child = new ChildProfile
        {
            ParentId = request.ParentId,
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            BloodType = request.BloodType,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(child, cancellationToken);

        return new CreateChildResult(child.Id, child.ParentId, child.FullName);
    }
}
