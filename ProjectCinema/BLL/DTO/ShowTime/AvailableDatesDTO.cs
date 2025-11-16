namespace ProjectCinema.BLL.DTO.ShowTime
{
    public class AvailableDatesDTO
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; } = null!;
        public List<DateOnly> AvailableDates { get; set; } = new();
    }
}

