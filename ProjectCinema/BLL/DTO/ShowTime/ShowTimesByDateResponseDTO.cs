namespace ProjectCinema.BLL.DTO.ShowTime
{
    public class ShowTimesByDateResponseDTO
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; } = null!;
        public DateOnly SelectedDate { get; set; }
        public List<ShowTimeByDateDTO> ShowTimes { get; set; } = new();
    }
}

