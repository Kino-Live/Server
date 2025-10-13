using FluentValidation;
using ProjectCinema.BLL.DTO.Cinema;

namespace ProjectCinema.Validations.CinemaValidation
{
    public class CinemaCreateDTOValidator : AbstractValidator<CinemaCreateDTO>
    {
        public CinemaCreateDTOValidator()
        {
            RuleFor(x => x.CinemaName)
                .NotEmpty().WithMessage("Cinema name is required.")
                .Length(2, 256).WithMessage("Cinema name must be between 2 and 256 characters.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Cinema name must contain only letters and spaces.");

            //RuleFor(x => x.Location)
            //    .NotEmpty().WithMessage("Location is required.")
            //    .Length(2, 256).WithMessage("Location must be between 2 and 256 characters.");
              
        }
    }
}
