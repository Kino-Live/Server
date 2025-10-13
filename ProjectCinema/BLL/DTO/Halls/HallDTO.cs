using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Halls
{
    public class HallDTO
    {
        public int HallId { get; set; }
        public string HallName { get; set; } = null!;
        public HallAvailability HallAvailability { get; set; }
        public int RowCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CinemaId { get; set; }
    }
}
