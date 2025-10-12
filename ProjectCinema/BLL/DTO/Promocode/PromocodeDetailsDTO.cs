using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ProjectCinema.BLL.DTO.Booking;

namespace ProjectCinema.BLL.DTO.Promocode
{
    public class PromocodeDetailsDTO
    {
        public int PromocodeId { get; set; }
        public string UniqueCode { get; set; } = null!;
        public decimal PromocodeAmount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<BookingDTO>? Bookings { get; set; }
    }
}
