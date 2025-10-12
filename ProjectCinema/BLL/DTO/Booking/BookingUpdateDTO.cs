using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Booking
{
    public class BookingUpdateDTO
    {
        public BookingStatus? BookingStatus { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than 0")]
        public decimal? TotalPrice { get; set; }

        public int? PromocodeId { get; set; }

        public int? PaymentId { get; set; }
    }
}
