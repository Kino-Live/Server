using FluentValidation;
using ProjectCinema.BLL.DTO.Users;

namespace ProjectCinema.Validations.UserValidation
{
    /// <summary>
    /// Validator for password reset confirmation
    /// </summary>
    public class PasswordResetConfirmDTOValidator : AbstractValidator<PasswordResetConfirmDTO>
    {
        public PasswordResetConfirmDTOValidator()
        {
            RuleFor(c => c.Token)
                .NotEmpty().WithMessage("Token is required")
                .MinimumLength(32).WithMessage("Token is invalid")
                .MaximumLength(512).WithMessage("Token is too long");

            RuleFor(c => c.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(10).WithMessage("Password must be at least 10 characters long")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character (@$!%*?&#)");

            RuleFor(c => c.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required")
                .Equal(c => c.NewPassword).WithMessage("Passwords do not match");
        }
    }
}

