using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCinema.Entities
{
    public class StreamingAccess
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StreamingAccessId { get; set; }

        public DateTime PurchaseDate { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public StreamingAccessStatus AccessStatus { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [ForeignKey("PaymentId")]
        public Payment? Payment { get; set; }

        public int PaymentId { get; set; }
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public int MovieId { get; set; }

        [ForeignKey("MovieId")]
        public Movie? Movie { get; set; }
    }
}
