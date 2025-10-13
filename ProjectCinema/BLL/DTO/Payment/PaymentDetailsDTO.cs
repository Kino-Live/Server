using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ProjectCinema.BLL.DTO.Booking;
using ProjectCinema.BLL.DTO.StreamingAccess;

namespace ProjectCinema.BLL.DTO.Payment
{
    public class PaymentDetailsDTO
    {
        public int PaymentId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal AmountPaid { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime PaidAt { get; set; }

        public BookingDTO? Booking { get; set; }
        public StreamingAccessDTO? StreamingAccess { get; set; }
    }
}
