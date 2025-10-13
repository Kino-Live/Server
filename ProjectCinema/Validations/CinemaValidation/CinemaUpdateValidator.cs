using FluentValidation;
using ProjectCinema.BLL.DTO.Cinema;

namespace ProjectCinema.Validations.CinemaValidation
{
    public class CinemaUpdateValidator : AbstractValidator<CinemaUpdateDTO>
    {
        public CinemaUpdateValidator()
        {
            When(c => c.CinemaName != null, () =>
                 RuleFor(x => x.CinemaName)
                    .Length(2, 256).WithMessage("Cinema name must be between 2 and 256 characters.")
                    .Matches(@"^[a-zA-Z\s]+$").WithMessage("Cinema name must contain only letters and spaces.")
            );

            //When(c => c.Location != null, () =>
            //     RuleFor(x => x.Location)
            //        .Length(2, 256).WithMessage("Location must be between 2 and 256 characters.")
            //);
        }
    }
}
