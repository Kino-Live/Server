using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.Entities
{
    public class Row
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RowId { get; set; }

        [Required]
        public int RowNumber { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [Required]
        public int HallId { get; set; }

        [ForeignKey("HallId")]
        public Hall? Hall { get; set; }

        [Required]
        public ICollection<Seat>? Seats { get; set; }
    }
}
