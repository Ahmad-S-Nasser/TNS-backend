using MediatR;

namespace TipsAndSteps.UserManagement.Application.Commands.ChangePassword;

/// <summary>
/// Change the current user's password. Requires the correct current password.
/// </summary>
public sealed record ChangePasswordCommand(
    string UserId,
    string CurrentPassword,
    string NewPassword) : IRequest<ChangePasswordResult>;

public sealed record ChangePasswordResult(bool Success, string? Message);
