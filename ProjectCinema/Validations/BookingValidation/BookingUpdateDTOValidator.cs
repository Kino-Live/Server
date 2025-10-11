
using FluentValidation;
using ProjectCinema.BLL.DTO.Booking;

namespace ProjectCinema.Validations.BookingValidation
{
    public class BookingUpdateDTOValidator : AbstractValidator<BookingUpdateDTO>
    {
        public BookingUpdateDTOValidator()
        {
            When(b => b.BookingStatus.HasValue, () =>
            {
                RuleFor(b => b.BookingStatus).IsInEnum();
            });
        }
    }
}