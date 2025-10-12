using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.BLL.DTO.Ticket;
using ProjectCinema.BLL.DTO.Promocode;
using ProjectCinema.BLL.DTO.Payment;
using ProjectCinema.BLL.DTO.Notification;

namespace ProjectCinema.BLL.DTO.Booking
{
    public class BookingDetailsDTO
    {
        public int BookingId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public int UserId { get; set; }
        public int? PromocodeId { get; set; }
        public int? PaymentId { get; set; }
       
        public UserDTO? User { get; set; }

        public PaymentDTO? Payment { get; set; }

        public PromocodeDTO? Promocode { get; set; }

        public ICollection<TicketDTO>? Tickets { get; set; }

        public ICollection<NotificationDTO>? Notifications { get; set; }
    }
}
