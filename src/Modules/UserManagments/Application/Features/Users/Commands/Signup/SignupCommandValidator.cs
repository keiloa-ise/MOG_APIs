using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Features.Users.Commands.Signup
{
    public class SignupCommandValidator : AbstractValidator<SignupCommand>
    {
        public SignupCommandValidator()
        {
            RuleFor(x => x.Request.Username)
                .NotEmpty().WithMessage("Username is required")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");

            RuleFor(x => x.Request.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.Request.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"\d").WithMessage("Password must contain at least one number")
                .Matches(@"[^\w\d\s]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.Request.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm Password is required")
                .Equal(x => x.Request.Password).WithMessage("Passwords do not match");

            RuleFor(x => x.Request.FullName)
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Request.FullName));

            RuleFor(x => x.Request.PhoneNumber)
                .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid phone number")
                .When(x => !string.IsNullOrEmpty(x.Request.PhoneNumber));
        }
    }
}
