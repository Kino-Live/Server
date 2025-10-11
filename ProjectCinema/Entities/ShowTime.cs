using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCinema.Entities
{
    public class ShowTime
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShowTimeId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public ViewingFormat ViewingFormat { get; set; }

        public ShowTimeStatus ShowTimeStatus { get; set; }
        
        [Required]
        [Range(1, double.MaxValue)]
        public decimal TicketPrice { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
        public ICollection<Ticket>? Tickets { get; set; }

        [ForeignKey("MovieScreeningId")]
        public MovieScreening? MovieScreening { get; set; }

        [Required]
        public int MovieScreeningId { get; set; }

        [ForeignKey("HallId")]
        public Hall? Hall { get; set; }

        [Required]
        public int HallId { get; set; }
        
    }
}
