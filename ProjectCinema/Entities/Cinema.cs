using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCinema.Entities
{
    public class Cinema
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CinemaId { get; set; }

        [StringLength(256, MinimumLength = 2)]
        [Required]
        public string? CinemaName { get; set; }

        [StringLength(256, MinimumLength = 2)]
        [Required]
        public string? Adress { get; set; }

        [Required]
        public string MapLocation { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<MovieScreening>? MovieScreenings { get; set; }

        public ICollection<Hall>? Halls { get; set; }
    }
}
