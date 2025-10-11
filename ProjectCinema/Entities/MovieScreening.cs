using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCinema.Entities
{
    public class MovieScreening
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MovieScreeningId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [Required]
        public MovieScreeningRelevance MovieScreeningRelevance { get; set; }

        [ForeignKey("MovieId")]
        public Movie? Movie { get; set; }

        [Required]
        public int MovieId { get; set; }

        [ForeignKey("CinemaId")]
        public Cinema? Cinema { get; set; }

        [Required]
        public int CinemaId { get; set; }

        public ICollection<ShowTime>? ShowTimes { get; set; }
    }
}
