using FluentValidation;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.Signin
{
    public class SigninCommandValidator : AbstractValidator<SigninCommand>
    {
        public SigninCommandValidator()
        {
            RuleFor(x => x.Request.UsernameOrEmail)
                .NotEmpty().WithMessage("Username or email is required")
                .MaximumLength(100).WithMessage("Username or email cannot exceed 100 characters");

            RuleFor(x => x.Request.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");
        }
    }
}
