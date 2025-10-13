using FluentValidation;
using ProjectCinema.BLL.DTO.Promocode;

namespace ProjectCinema.Validations.PromocodeValidation
{
    public class PromocodeCreateDTOValidator : AbstractValidator<PromocodeCreateDTO>
    {
        public PromocodeCreateDTOValidator()
        {
            RuleFor(x => x.UniqueCode)
                    .NotEmpty().WithMessage("Unique code is required.")
                    .Length(5, 50).WithMessage("Unique code must be between 5 and 50 characters."); 


            RuleFor(x => x.PromocodeAmount)
                .GreaterThan(0).When(x => x.PromocodeAmount != null)
                .WithMessage("Promocode amount must be greater than 0 if specified.");
        }
    }
}
