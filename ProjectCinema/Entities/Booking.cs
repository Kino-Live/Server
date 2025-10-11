using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCinema.Entities
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [Required]
        public BookingStatus BookingStatus { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
        [Required]
        public int UserId { get; set; }

        public Payment? Payment { get; set; }

        [ForeignKey("PromocodeId")]
        public Promocode? Promocode { get; set; }

        [Required]
        public int PromocodeId { get; set; }
        public ICollection<Ticket>? Tickets { get; set; }
    }
}
