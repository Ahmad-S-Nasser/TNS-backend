using MediatR;
using TipsAndSteps.UserManagement.Application.Abstractions;

namespace TipsAndSteps.UserManagement.Application.Commands.UpdateProfile;

public sealed class UpdateProfileHandler(IUserRepository repo)
    : IRequestHandler<UpdateProfileCommand, bool>
{
    public async Task<bool> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var user = await repo.FindByIdAsync(request.UserId, ct);
        if (user is null) return false;

        if (request.FirstName is not null)        user.FirstName        = request.FirstName;
        if (request.LastName is not null)         user.LastName         = request.LastName;
        if (request.PhoneNumber is not null)      user.PhoneNumber      = request.PhoneNumber;
        if (request.PreferredLanguage is not null) user.PreferredLanguage = request.PreferredLanguage;

        await repo.UpdateAsync(user, ct);
        return true;
    }
}
