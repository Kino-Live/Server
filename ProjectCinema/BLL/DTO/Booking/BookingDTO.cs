using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.Enums;

namespace ProjectCinema.BLL.DTO.Booking
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public int UserId { get; set; }
        public int? PromocodeId { get; set; }
        public int? PaymentId { get; set; }
    }
}
