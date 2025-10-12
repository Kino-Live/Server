using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCinema.Entities
{
    public class Seat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SeatId { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int SeatNumber { get; set; }

        public SeatAvailability SeatAvailability { get; set; }

        [Required]
        public SeatType SeatType { get; set; } 

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<Ticket>? Tickets { get; set; }

        [Required]
        public int RowId { get; set; }

        [ForeignKey("RowId")]
        public Row? Row { get; set; }
    }
}
