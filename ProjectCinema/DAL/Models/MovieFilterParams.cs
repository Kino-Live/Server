namespace ProjectCinema.DAL.Models
{
    public class MovieFilterParams
    {
        public IEnumerable<string>? Genres { get; init; }
        public int? YearFrom { get; init; }
        public int? YearTo { get; init; }
        public double? RatingMin { get; init; }
        public double? RatingMax { get; init; }
    }
}
