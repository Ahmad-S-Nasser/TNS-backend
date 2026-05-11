using MediatR;

namespace TipsAndSteps.UserManagement.Application.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;

public record LoginResult(
    bool Success, 
    string? Token = null, 
    string? Message = null,
    string? UserId = null,
    string? Email = null,
    string? Role = null);
