using ProjectCinema.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ProjectCinema.BLL.DTO.Seat;
using ProjectCinema.BLL.DTO.Halls;

namespace ProjectCinema.BLL.DTO.Row
{
    public class RowDetailsDTO
    {
        public int RowId { get; set; }
        public int RowNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int HallId { get; set; }
        public HallDTO? Hall { get; set; }

        public ICollection<SeatDTO>? Seats { get; set; }
    }
}
