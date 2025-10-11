using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.ShowTime
{
    public class ShowTimeUpdateDTO
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ViewingFormat? ViewingFormat { get; set; }
        public ShowTimeStatus? ShowTimeStatus { get; set; }
        public Decimal? TicketPrice { get; set; }
        public int HallId { get; set; }
    }
}
