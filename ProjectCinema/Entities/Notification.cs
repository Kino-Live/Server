
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCinema.Entities
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationId { get; set; }

        [Required]
        [StringLength(1028, MinimumLength = 1)]
        public string Message { get; set; } = null!;

        [Required]
        public DateTime ScheduledTime { get; set; }

        public DateTime SentTime { get; set; }

        public bool IsSent { get; set; }

        [Required]
        public NotificationChannel Channel { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }


    }
}
