using FluentValidation;
using ProjectCinema.BLL.DTO.Payment;

namespace ProjectCinema.Validations.PaymentValidation
{
    public class PaymentUpdateDTOValidator : AbstractValidator<PaymentUpdateDTO>
    {
        public PaymentUpdateDTOValidator()
        {
            When(p => p.PaymentMethod.HasValue, () =>
            {
                RuleFor(p => p.PaymentMethod)
                    .IsInEnum().WithMessage("The specified payment method is incorrect.");
            });
        }
    }
}
