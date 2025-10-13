using FluentValidation;
using ProjectCinema.BLL.DTO.Booking;
using ProjectCinema.BLL.DTO.Payment;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.BLL.Interfaces.IMovieScreeningServices;
using ProjectCinema.Enums;
using ProjectCinema.Repositories.Interfaces;

namespace ProjectCinema.Validations.PaymentValidation
{
    public class PaymentCreateDTOValidator : AbstractValidator<PaymentCreateDTO>
    {
        private readonly IBookingService _bookingService;
        private readonly IPaymentService _paymanentService;
        public PaymentCreateDTOValidator(IBookingService bookingService, IPaymentService paymentService)
        {

            _bookingService = bookingService;
            _paymanentService = paymentService;


            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Payment method is required.")
                .IsInEnum().WithMessage("The specified payment method is incorrect.");

            RuleFor(x => x.AmountPaid)
                .GreaterThan(0).WithMessage("Amount paid must be greater than 0.");

            //RuleFor(x => x)
            //    .MustAsync((x, cancellationToken) => IsEqualToBookingAmount(x.AmountPaid, x.BookingId, cancellationToken))
            //    .WithMessage("Payment amount must match the booking amount.");
        }

        //private async Task<bool> IsValidBooking(int bookingId, CancellationToken cancellationToken)
        //{
        //    BookingDTO booking = await _bookingService.GetByIdAsync(bookingId);

        //    return booking != null;
        //}

        //private async Task<bool> IsUniquePayment(int bookingId, CancellationToken cancellationToken)
        //{
        //    PaymentDTO existingPayment = await _paymanentService.GetByIdAsync(bookingId);

        //    return existingPayment == null || existingPayment.PaymentStatus != PaymentStatus.Success;
        //}

        //private async Task<bool> IsEqualToBookingAmount(decimal amountPaid, int bookingId, CancellationToken cancellationToken)
        //{
        //    BookingDTO booking = await _bookingService.GetByIdAsync(bookingId);

        //    return booking?.TotalPrice == amountPaid;
        //}
    }
}
