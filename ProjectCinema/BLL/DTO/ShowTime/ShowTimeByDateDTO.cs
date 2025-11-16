using ProjectCinema.Enums;

namespace ProjectCinema.BLL.DTO.ShowTime
{
    public class ShowTimeByDateDTO
    {
        public int ShowTimeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ViewingFormat ViewingFormat { get; set; }
        public decimal TicketPrice { get; set; }
        public int HallId { get; set; }
        public string HallName { get; set; } = null!;
        public int CinemaId { get; set; }
        public string CinemaName { get; set; } = null!;
        public string CinemaAddress { get; set; } = null!;
        public ShowTimeStatus ShowTimeStatus { get; set; }
    }
}

