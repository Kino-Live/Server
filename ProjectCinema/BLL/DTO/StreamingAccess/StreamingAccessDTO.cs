using ProjectCinema.Enums;

namespace ProjectCinema.BLL.DTO.StreamingAccess
{
    public class StreamingAccessDTO
    {
        public int StreamingAccessId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public StreamingAccessStatus AccessStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? PaymentId { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
    }
}
