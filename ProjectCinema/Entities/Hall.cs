using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ProjectCinema.Enums;
using ProjectCinema.Entities;

namespace ProjectCinema.Entities
{
    public class Hall
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int HallId { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 2)]
        public string HallName { get; set; } = null!;

        public HallAvailability HallAvailability { get; set; }
        [Required]
        public int RowCount {  get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("CinemaId")]
        public Cinema? Cinema { get; set; }
        [Required]
        public int CinemaId { get; set; }
        public ICollection<ShowTime>? ShowTimes { get; set; }

        public ICollection<Row>? Rows { get; set; }
    }
}