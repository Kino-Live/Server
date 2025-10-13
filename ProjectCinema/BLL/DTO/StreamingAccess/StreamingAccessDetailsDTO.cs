using ProjectCinema.Enums;
using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.BLL.DTO.Movie;
using ProjectCinema.BLL.DTO.Payment;

namespace ProjectCinema.BLL.DTO.StreamingAccess
{
    public class StreamingAccessDetailsDTO
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

        public UserDTO? User { get; set; }
        public MovieDTO? Movie { get; set; }
        public PaymentDTO? Payment { get; set; }
    }
}
