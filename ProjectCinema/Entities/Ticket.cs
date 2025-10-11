using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace ProjectCinema.Entities
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TicketId { get; set; }

        [Required]
        public TicketStatus TicketStatus { get; set; }

        [Required]
        public decimal PriceAtPurchase { get; set; }

        [Required]
        public string QRCode { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [ForeignKey("ShowTimeId")]
        public ShowTime? ShowTime { get; set; }

        [Required]
        public int ShowTimeId { get; set; }

        [ForeignKey("SeatId")]
        public Seat? Seat { get; set; }

        [Required]
        public int SeatId { get; set; }

        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }

        public int BookingId { get; set; }
    }
}
