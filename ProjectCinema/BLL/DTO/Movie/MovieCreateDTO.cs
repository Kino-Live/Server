using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Movie
{
    public class MovieCreateDTO
    {
        [Required]
        [StringLength(256, MinimumLength = 1)]
        public string MovieName { get; set; } = null!;

        [Required]
        [StringLength(1028, MinimumLength = 1)]
        public string Description { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0")]
        public int DurationInMinutes { get; set; }

        [Required]
        [Range(0, 21, ErrorMessage = "Age restriction must be between 0 and 21")]
        public int AgeRestriction { get; set; }

        [Required]
        public DateTime GlobalStartDate { get; set; }

        [Required]
        public DateTime GlobalEndDate { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 1)]
        public string Url { get; set; } = null!;

        [Required]
        public DateOnly ReleaseYear { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 2)]
        public string Genre { get; set; } = null!;

        [Required]
        [StringLength(64, MinimumLength = 2)]
        public string Language { get; set; } = null!;

        [StringLength(256, MinimumLength = 1)]
        public string? ProductionStudio { get; set; }

        [StringLength(256, MinimumLength = 1)]
        public string? Director { get; set; }

        public string? MainCast { get; set; }

        [Required]
        public StatusOfMovie Status { get; set; }
    }
}
