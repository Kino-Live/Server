using FluentValidation;
using ProjectCinema.BLL.DTO.Booking;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.Enums;

namespace ProjectCinema.Validations.BookingValidation
{
    public class BookingCreateDTOValidator : AbstractValidator<BookingCreateDTO>
    {
        public BookingCreateDTOValidator(IUserService userService,
                                         IPromocodeService promocodeService,
                                         IPaymentService paymentService)
        {
            RuleFor(x => x.UserId)
                .MustAsync(async (userId, ct) => await userService.GetByIdAsync(userId) != null)
                .WithMessage(x => $"User with id equals {x.UserId} does not exist.");

            When(x => x.PromocodeId.HasValue, () =>
            {
                RuleFor(x => x.PromocodeId.Value)
                    .MustAsync(async (promoId, ct) =>
                    {
                        var promo = await promocodeService.GetByIdAsync(promoId);
                        return promo != null;
                    })
                    .WithMessage(x => $"Promocode with id equals {x.PromocodeId} does not exist.");

            });
        }
    }
}
