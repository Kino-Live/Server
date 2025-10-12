using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Notification
{
    public class NotificationUpdateDTO
    {
        [StringLength(1028, MinimumLength = 1)]
        public string? Message { get; set; }

        public DateTime? ScheduledTime { get; set; }

        public DateTime? SentTime { get; set; }

        public bool? IsSent { get; set; }

        public NotificationChannel? Channel { get; set; }

        public NotificationType? Type { get; set; }
    }
}
