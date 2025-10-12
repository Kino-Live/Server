using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Movie
{
    public class MovieDTO
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int DurationInMinutes { get; set; }
        public int AgeRestriction { get; set; }
        public DateTime GlobalStartDate { get; set; }
        public DateTime GlobalEndDate { get; set; }
        public string Url { get; set; } = null!;
        public DateOnly ReleaseYear { get; set; }
        public string Genre { get; set; } = null!;
        public string Language { get; set; } = null!;
        public string? ProductionStudio { get; set; }
        public string? Director { get; set; }
        public string? MainCast { get; set; }
        public StatusOfMovie Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
