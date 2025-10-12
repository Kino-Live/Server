using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Movie
{
    public class MovieUpdateDTO
    {
        [StringLength(256, MinimumLength = 1)]
        public string? MovieName { get; set; }

        [StringLength(1028, MinimumLength = 1)]
        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0")]
        public int? DurationInMinutes { get; set; }

        [Range(0, 21, ErrorMessage = "Age restriction must be between 0 and 21")]
        public int? AgeRestriction { get; set; }

        public DateTime? GlobalStartDate { get; set; }

        public DateTime? GlobalEndDate { get; set; }

        [StringLength(256, MinimumLength = 1)]
        public string? Url { get; set; }

        public DateOnly? ReleaseYear { get; set; }

        [StringLength(128, MinimumLength = 2)]
        public string? Genre { get; set; }

        [StringLength(64, MinimumLength = 2)]
        public string? Language { get; set; }

        [StringLength(256, MinimumLength = 1)]
        public string? ProductionStudio { get; set; }

        [StringLength(256, MinimumLength = 1)]
        public string? Director { get; set; }

        public string? MainCast { get; set; }

        public StatusOfMovie? Status { get; set; }
    }
}
