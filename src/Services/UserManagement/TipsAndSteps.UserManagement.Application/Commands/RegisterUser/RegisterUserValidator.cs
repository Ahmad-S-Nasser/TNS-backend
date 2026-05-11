using FluentValidation;

namespace TipsAndSteps.UserManagement.Application.Commands.RegisterUser;

public sealed class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress()
            .WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Invalid user role.");
    }
}
