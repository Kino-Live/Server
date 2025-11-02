using FluentValidation;
using ProjectCinema.BLL.DTO.Users;

namespace ProjectCinema.Validations.UserValidation
{
    /// <summary>
    /// Validator for user login data
    /// </summary>
    public class LoginUserDTOValidator : AbstractValidator<LoginUserDTO>
    {
        public LoginUserDTOValidator()
        {
            RuleFor(l => l.UsernameOrEmail)
                .NotEmpty().WithMessage("Username or Email is required")
                .Length(2, 256).WithMessage("Username or Email must be between 2 and 256 characters");

            RuleFor(l => l.Password)
                .NotEmpty().WithMessage("Password is required")
                .Length(6, 100).WithMessage("Password must be between 6 and 100 characters");
        }
    }
}
