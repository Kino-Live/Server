using FluentValidation;
using ProjectCinema.BLL.DTO.Promocode;
using ProjectCinema.Enums;

namespace ProjectCinema.Validations.PromocodeValidation
{
    public class PromocodeUpdateValidator : AbstractValidator<PromocodeUpdateDTO>
    {
        public PromocodeUpdateValidator()
        {
            When(p => p.UniqueCode != null, () =>
                RuleFor(x => x.UniqueCode)
                    .NotEmpty().WithMessage("Unique code is required.")
                    .Length(3, 64).WithMessage("Unique code must be between 3 and 64 characters.")
            );


            When(p => p.PromocodeAmount.HasValue, () =>
                RuleFor(x => x.PromocodeAmount)
                    .GreaterThan(0).WithMessage("Amount must be greater than 0.")
            );

        }
    }
}
