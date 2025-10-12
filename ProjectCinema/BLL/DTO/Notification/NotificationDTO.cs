using ProjectCinema.Enums;

namespace ProjectCinema.BLL.DTO.Notification
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }
        public string Message { get; set; } = null!;
        public DateTime ScheduledTime { get; set; }
        public DateTime SentTime { get; set; }
        public bool IsSent { get; set; }
        public NotificationChannel Channel { get; set; }
        public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int BookingId { get; set; }
    }
}
