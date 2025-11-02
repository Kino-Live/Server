using FluentValidation;
using ProjectCinema.BLL.DTO.Users;

namespace ProjectCinema.Validations.UserValidation
{
    /// <summary>
    /// Validator for user registration data
    /// </summary>
    public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterDTOValidator()
        {
            RuleFor(r => r.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .Length(2, 256).WithMessage("First name must be between 2 and 256 characters")
                .Matches(@"^[A-Za-zА-Яа-яёЁ\s\-']+$").WithMessage("First name contains invalid characters");

            RuleFor(r => r.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .Length(2, 256).WithMessage("Last name must be between 2 and 256 characters")
                .Matches(@"^[A-Za-zА-Яа-яёЁ\s\-']+$").WithMessage("Last name contains invalid characters");

            RuleFor(r => r.Username)
                .NotEmpty().WithMessage("Username is required")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters")
                .Matches(@"^[A-Za-z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(256).WithMessage("Email is too long");

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password is required")
                .Length(6, 100).WithMessage("Password must be between 6 and 100 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number");

            RuleFor(r => r.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required")
                .Equal(r => r.Password).WithMessage("Passwords do not match");

            RuleFor(r => r.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").When(r => !string.IsNullOrEmpty(r.PhoneNumber))
                .WithMessage("Invalid phone number format");

            RuleFor(r => r.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateOnly.FromDateTime(DateTime.Today.AddYears(-13)))
                .WithMessage("User must be at least 13 years old")
                .GreaterThan(new DateOnly(1900, 1, 1))
                .WithMessage("Date of birth cannot be earlier than 1900");
        }
    }
}
