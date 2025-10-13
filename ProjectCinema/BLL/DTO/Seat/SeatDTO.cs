using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Seat
{
    public class SeatDTO
    {
        public int SeatId { get; set; }
        public int SeatNumber { get; set; }
        public SeatAvailability SeatAvailability { get; set; }
        public SeatType SeatType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int RowId { get; set; }
    }
}
