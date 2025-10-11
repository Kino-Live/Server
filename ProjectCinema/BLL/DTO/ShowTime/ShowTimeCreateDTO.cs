using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.ShowTime
{
    public class ShowTimeCreateDTO
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ViewingFormat ViewingFormat { get; set; }
        public Decimal TicketPrice { get; set; }
        public int MovieScreeningId { get; set; }
        public int HallId { get; set; }
    }
}
