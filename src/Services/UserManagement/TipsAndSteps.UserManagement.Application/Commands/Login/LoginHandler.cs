using MediatR;
using TipsAndSteps.UserManagement.Application.Abstractions;
using BCryptNet = BCrypt.Net.BCrypt;

namespace TipsAndSteps.UserManagement.Application.Commands.Login;

public sealed class LoginHandler(
    IUserReadRepository userRepo,
    IJwtProvider jwtProvider) : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepo.FindByEmailAsync(request.Email, cancellationToken);

        if (user == null)
        {
            return new LoginResult(false, Message: "Invalid email or password");
        }

        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            Console.WriteLine($"[Login] User {user.Email} has no password hash in DB!");
            return new LoginResult(false, Message: "Account not properly initialized. Please contact support.");
        }

        if (!BCryptNet.Verify(request.Password, user.PasswordHash))
        {
            return new LoginResult(false, Message: "Invalid email or password");
        }

        if (!user!.IsActive)
        {
            return new LoginResult(false, Message: "Account is deactivated");
        }

        var token = jwtProvider.Generate(user);

        return new LoginResult(
            true, 
            Token: token, 
            UserId: user.Id, 
            Email: user.Email, 
            Role: user.Role.ToString());
    }
}
