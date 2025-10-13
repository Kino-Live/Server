using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Ticket
{
    public class TicketCreateDTO
    {
        [Required]
        public TicketStatus TicketStatus { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal PriceAtPurchase { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "QR Code must be between 1 and 500 characters")]
        public string QRCode { get; set; } = null!;

        [Required]
        public int ShowTimeId { get; set; }

        [Required]
        public int SeatId { get; set; }

        [Required]
        public int BookingId { get; set; }
    }
}
