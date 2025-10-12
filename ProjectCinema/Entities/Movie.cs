using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCinema.Entities
{
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MovieId { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 1)]
        public string MovieName { get; set; } = null!;

        [Required]
        [StringLength(1028, MinimumLength = 1)]
        public string Description { get; set; } = null!;

        [Range(1, int.MaxValue)]
        [Required]
        public int DurationInMinutes { get; set; }

        [Range(0, 21)]
        [Required]
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

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<MovieScreening>? MovieScreenings { get; set; }

        public ICollection<Review>? Reviews { get; set; }

        public ICollection<StreamingAccess>? StreamingAccesses { get; set; }
       
    }
}
