using FluentValidation;
using GymBLL.ModelVM.Identity;

namespace GymBLL.Validation
{
    public class PasswordVMValidator : AbstractValidator<PasswordVM>
    {
        public PasswordVMValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email address")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(4).WithMessage("Password must be at least 4 characters long");
                //.Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                //.Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                //.Matches(@"\d").WithMessage("Password must contain at least one number")
                //.Matches(@"[@$!%*?&]").WithMessage("Password must contain at least one special character (@$!%*?&)");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Please confirm your new password")
                .Equal(x => x.NewPassword).WithMessage("The new password and confirmation password do not match");
        }
    }

    public class ForgotPasswordVMValidator : AbstractValidator<ForgotPasswordVM>
    {
        public ForgotPasswordVMValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");
        }
    }

    public class ResetPasswordVMValidator : AbstractValidator<ResetPasswordVM>
    {
        public ResetPasswordVMValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(4).WithMessage("Password must be at least 4 characters long");
                //.Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                //.Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                //.Matches(@"\d").WithMessage("Password must contain at least one number")
                //.Matches(@"[@$!%*?&]").WithMessage("Password must contain at least one special character (@$!%*?&)");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Please confirm your password")
                .Equal(x => x.Password).WithMessage("The password and confirmation password do not match");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Reset token is required");
        }
    }
}
