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

        [Range(1, double.MaxValue, ErrorMessage = "Ticket price must be greater than 0")]
        public decimal? TicketPrice { get; set; }

    }
}
