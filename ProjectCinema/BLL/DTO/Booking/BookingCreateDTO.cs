using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Booking
{
    public class BookingCreateDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than 0")]
        public decimal TotalPrice { get; set; }

        public int? PromocodeId { get; set; }

        // Note: PaymentId will be null initially
        // Booking starts as "Pending" status
        // Payment is processed separately and linked later
    }
}
