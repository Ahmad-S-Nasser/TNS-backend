using BCrypt.Net;
using MediatR;
using TipsAndSteps.UserManagement.Application.Abstractions;

namespace TipsAndSteps.UserManagement.Application.Commands.ChangePassword;

public sealed class ChangePasswordHandler(IUserRepository repo)
    : IRequestHandler<ChangePasswordCommand, ChangePasswordResult>
{
    public async Task<ChangePasswordResult> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var user = await repo.FindByIdAsync(request.UserId, ct);
        if (user is null)
            return new ChangePasswordResult(false, "User not found.");

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return new ChangePasswordResult(false, "Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await repo.UpdateAsync(user, ct);

        return new ChangePasswordResult(true, null);
    }
}
