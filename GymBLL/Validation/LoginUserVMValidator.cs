using FluentValidation;
using GymBLL.ModelVM.Identity;

namespace GymBLL.Validation
{
    public class LoginUserVMValidator : AbstractValidator<LoginUserVM>
    {
        public LoginUserVMValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
