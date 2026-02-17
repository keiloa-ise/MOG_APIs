using FluentValidation;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Queries.CheckAvailability
{
    public class CheckAvailabilityQueryValidator : AbstractValidator<CheckAvailabilityQuery>
    {
        public CheckAvailabilityQueryValidator()
        {
            RuleFor(x => x.Request)
                .Must(x => !string.IsNullOrEmpty(x.Email) || !string.IsNullOrEmpty(x.Username))
                .WithMessage("Please provide either email or username");

            When(x => !string.IsNullOrEmpty(x.Request.Email), () =>
            {
                RuleFor(x => x.Request.Email)
                    .EmailAddress().WithMessage("Invalid email address");
            });

            When(x => !string.IsNullOrEmpty(x.Request.Username), () =>
            {
                RuleFor(x => x.Request.Username)
                    .Length(3, 50).WithMessage("Username must be between 3 and 50 characters")
                    .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");
            });
        }
    }
}
