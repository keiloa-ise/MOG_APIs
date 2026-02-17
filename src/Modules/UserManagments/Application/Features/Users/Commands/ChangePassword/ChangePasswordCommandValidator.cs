using FluentValidation;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.Request.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required")
                .MinimumLength(6).WithMessage("Current password must be at least 6 characters");

            RuleFor(x => x.Request.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(8).WithMessage("New password must be at least 8 characters")
                .Matches(@"[A-Z]").WithMessage("New password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("New password must contain at least one lowercase letter")
                .Matches(@"\d").WithMessage("New password must contain at least one number")
                .Matches(@"[^\w\d\s]").WithMessage("New password must contain at least one special character")
                .NotEqual(x => x.Request.CurrentPassword).WithMessage("New password must be different from current password");

            RuleFor(x => x.Request.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required")
                .Equal(x => x.Request.NewPassword).WithMessage("Passwords do not match");

            RuleFor(x => x.CurrentUserId)
                .GreaterThan(0).WithMessage("Invalid user");
        }
    }
}
