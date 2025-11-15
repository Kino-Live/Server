namespace ProjectCinema.BLL.DTO.Movie
{
    public class MovieListItemDTO
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; } = null!;
        public int ReleaseYear { get; set; }
        public string Genre { get; set; } = null!;
        public string Language {  get; set; } = null!;
        public string Url { get; set; } = null!;
        public int AgeRestriction { get; set; }
    }
}
