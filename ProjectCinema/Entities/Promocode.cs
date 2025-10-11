using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCinema.Entities
{
    public class Promocode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PromocodeId { get; set; }

        [Required]
        public string UniqueCode { get; set; } = null!;

        [Required]
        [Range(1, 100)]
        public decimal PromocodeAmount { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAdt { get; set; }

        public ICollection<Booking>? Bookings { get; set; }

    }
}
