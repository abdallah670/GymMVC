using FluentValidation;
using GymBLL.ModelVM;

namespace GymBLL.Validation
{
    public class TempRegistrationVMValidator : AbstractValidator<TempRegistrationVM>
    {
        public TempRegistrationVMValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.OtpCode)
                .Length(6).WithMessage("OTP code must be 6 digits")
                .Matches(@"^\d+$").WithMessage("OTP code must contain only digits")
                .When(x => !string.IsNullOrEmpty(x.OtpCode));

            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.LastName));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Height)
                .InclusiveBetween(50, 300).WithMessage("Height must be between 50 and 300 cm")
                .When(x => x.Height.HasValue);

            RuleFor(x => x.Weight)
                .InclusiveBetween(20, 500).WithMessage("Weight must be between 20 and 500 kg")
                .When(x => x.Weight.HasValue);
        }
    }
}
