using FluentValidation;
using ProjectCinema.BLL.DTO.Ticket;
using ProjectCinema.Enums;

namespace ProjectCinema.Validations.TicketValidation
{
    public class TicketUpdateDTOValidator : AbstractValidator<TicketUpdateDTO>
    {
        public TicketUpdateDTOValidator()
        {
            When(t => t.TicketStatus.HasValue, () =>
            {
                RuleFor(t => t.TicketStatus)
                .IsInEnum()
                .WithMessage("Is not valid data");
            });

            When(t => t.PriceAtPurchase.HasValue, () =>
            {
                RuleFor(x => x.PriceAtPurchase)
                    .GreaterThan(0)
                    .WithMessage("Ticket price must be greater than 0.");
            });
        }
    }
}