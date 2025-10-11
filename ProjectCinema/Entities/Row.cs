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
        public int HallId { get; set; }
        [ForeignKey("HallId")]
        public Hall? Hall { get; set; }
        [Required]
        public int RowNumber { get; set; }

        public ICollection<Seat>? Seats { get; set; }
    }
}
