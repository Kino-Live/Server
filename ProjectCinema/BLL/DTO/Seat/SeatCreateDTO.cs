using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Seat
{
    public class SeatCreateDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Seat number must be greater than 0")]
        public int SeatNumber { get; set; }

        [Required]
        public SeatAvailability SeatAvailability { get; set; }

        [Required]
        public SeatType SeatType { get; set; }

        [Required]
        public int RowId { get; set; }
    }
}
