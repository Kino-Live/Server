using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProjectCinema.BLL.DTO.Ticket;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.Data;
using ProjectCinema.Enums;

namespace ProjectCinema.Validations.TicketValidation
{
    public class TicketCreateDTOValidator : AbstractValidator<TicketCreateDTO>
    {
        private readonly IShowTimeService _showTimeService;
        private readonly ISeatService _seatService;
        private readonly IBookingService _bookingService;
        private readonly AplicationDBContext _context;

        public TicketCreateDTOValidator(
            IShowTimeService showTimeService,
            ISeatService seatService,
            IBookingService bookingService,
            AplicationDBContext context)
        {
            _showTimeService = showTimeService;
            _seatService = seatService;
            _bookingService = bookingService;
            _context = context;

            RuleFor(x => x.PriceAtPurchase)
                .NotEmpty()
                .WithMessage("Price at purchase is a required field")
                .GreaterThan(0)
                .WithMessage("Ticket price must be greater than 0.");

            RuleFor(x => x.ShowTimeId)
                .NotEmpty()
                .WithMessage("ShowTime is a required field")
                .MustAsync(ShowTimeExists)
                .WithMessage(x => $"ShowTime with id {x.ShowTimeId} does not exist.");

            RuleFor(x => x.SeatId)
                .NotEmpty()
                .WithMessage("Seat is a required field")
                .MustAsync(SeatExists)
                .WithMessage(x => $"Seat with id {x.SeatId} does not exist.");

            RuleFor(x => x.BookingId)
                .NotEmpty()
                .WithMessage("Booking at purchase is a required field")
                .MustAsync(BookingExists)
                .WithMessage(x => $"Booking with id {x.BookingId} does not exist.");

            RuleFor(x => x)
                .MustAsync(SeatNotTaken)
                .WithMessage("This seat is already taken for this showtime.");

            RuleFor(x => x)
                .MustAsync(SeatAvailable)
                .WithMessage("This seat is not available for booking.");

            RuleFor(x => x)
                .MustAsync(ShowTimeNotInPast)
                .WithMessage("You cannot sell a ticket for a past showtime.");
        }

        private async Task<bool> ShowTimeExists(int showTimeId, CancellationToken ct)
        {
            return await _showTimeService.GetByIdAsync(showTimeId) != null;
        }

        private async Task<bool> SeatExists(int seatId, CancellationToken ct)
        {
            return await _seatService.GetByIdAsync(seatId) != null;
        }

        private async Task<bool> BookingExists(int bookingId, CancellationToken ct)
        {
            return await _bookingService.GetByIdAsync(bookingId) != null;
        }

        private async Task<bool> SeatNotTaken(TicketCreateDTO dto, CancellationToken ct)
        {
            return !await _context.Tickets
                .AnyAsync(t =>
                    t.ShowTimeId == dto.ShowTimeId &&
                    t.SeatId == dto.SeatId &&
                    t.TicketStatus == TicketStatus.Active,
                    ct);
        }

        private async Task<bool> SeatAvailable(TicketCreateDTO dto, CancellationToken ct)
        {
            var seat = await _seatService.GetByIdAsync(dto.SeatId);
            return seat != null && seat.SeatAvailability == SeatAvailability.Available;
        }

        private async Task<bool> ShowTimeNotInPast(TicketCreateDTO dto, CancellationToken ct)
        {
            var showTime = await _showTimeService.GetByIdAsync(dto.ShowTimeId);
            return showTime != null && showTime.StartTime >= DateTime.Now;
        }
    }
}
