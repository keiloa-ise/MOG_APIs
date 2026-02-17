using FluentValidation;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.ChangeUserRole
{
    public class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
    {
        public ChangeUserRoleCommandValidator()
        {
            RuleFor(x => x.Request.UserId)
                .GreaterThan(0).WithMessage("User ID must be greater than 0");

            RuleFor(x => x.Request.NewRoleId)
                .GreaterThan(0).WithMessage("Role ID must be greater than 0");

            RuleFor(x => x.Request.UserId)
                .NotEqual(x => x.CurrentUserId)
                .When(x => x.Request.UserId > 0 && x.CurrentUserId > 0)
                .WithMessage("Cannot change your own role");

            RuleFor(x => x.Request.Reason)
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Request.Reason));
        }
    }
}
