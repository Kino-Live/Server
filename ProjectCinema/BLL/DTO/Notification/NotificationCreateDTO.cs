using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Notification
{
    public class NotificationCreateDTO
    {
        [Required]
        [StringLength(1028, MinimumLength = 1)]
        public string Message { get; set; } = null!;

        [Required]
        public DateTime ScheduledTime { get; set; }

        [Required]
        public NotificationChannel Channel { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        [Required]
        public int BookingId { get; set; }
    }
}
