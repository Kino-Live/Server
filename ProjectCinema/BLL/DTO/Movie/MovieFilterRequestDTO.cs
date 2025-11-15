namespace ProjectCinema.BLL.DTO.Movie
{
    public sealed class MovieFilterRequestDTO
    {
        public IEnumerable<string>? Genres { get; init; }
        public int? YearFrom { get; init; }
        public int? YearTo { get; init; }
        public double? RatingMin {  get; init; }
        public double? RatingMax { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
