using ProjectCinema.BLL.DTO.Booking;
using ProjectCinema.BLL.DTO.Seat;
using ProjectCinema.BLL.DTO.ShowTime;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCinema.BLL.DTO.Ticket
{
    public class TicketDTO
    {
        public int TicketId { get; set; }
        public TicketStatus TicketStatus { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public string QRCode { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ShowTimeId { get; set; }
        public int SeatId { get; set; }
        public int BookingId { get; set; }

        public ShowTimeDTO? ShowTime { get; set; }
        public SeatDTO? Seat { get; set; }
        public BookingDTO? Booking { get; set; }
    }
}
