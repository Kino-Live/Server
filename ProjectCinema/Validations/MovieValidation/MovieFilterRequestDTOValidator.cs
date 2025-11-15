using FluentValidation;
using ProjectCinema.BLL.DTO.Movie;

namespace ProjectCinema.Validations.MovieValidation
{
    public class MovieFilterRequestDTOValidator : AbstractValidator<MovieFilterRequestDTO>
    {
        public MovieFilterRequestDTOValidator()
        {
            RuleFor(x => x.YearFrom)
                .GreaterThanOrEqualTo(1895)
                .When(x => x.YearFrom.HasValue)
                .WithMessage("YearFrom must be greater than or equal to 1895");

            RuleFor(x => x.YearTo)
                .LessThanOrEqualTo(DateTime.Now.Year)
                .When(x => x.YearTo.HasValue)
                .WithMessage("YearTo cannot be greater than the current year");

            RuleFor(x => x)
                .Must(x => !x.YearFrom.HasValue || !x.YearTo.HasValue || x.YearFrom <= x.YearTo)
                .WithMessage("YearFrom must be less than or equal to YearTo");

            RuleFor(x => x.RatingMin)
                .GreaterThanOrEqualTo(0)
                .When(x => x.RatingMin.HasValue)
                .WithMessage("RatingMin cannot be negative");

            RuleFor(x => x.RatingMax)
                .LessThanOrEqualTo(10)
                .When(x => x.RatingMax.HasValue)
                .WithMessage("RatingMax cannot be greater than 10");

            RuleFor(x => x)
                .Must(x => !x.RatingMin.HasValue || !x.RatingMax.HasValue || x.RatingMin <= x.RatingMax)
                .WithMessage("RatingMin must be less than or equal to RatingMin");

                RuleForEach(x => x.Genres)
                .NotEmpty().WithMessage("Genre name cannot be empty")
                .MaximumLength(128).WithMessage("Genre name must be less or equal to 128 characters")
                .Matches(@"^[A-Za-z\s\-]+$").WithMessage("Genre name contains invalid symbols");
        }
    }
}
