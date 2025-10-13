using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.StreamingAccess
{
    public class StreamingAccessUpdateDTO
    {
        public DateTime? PurchaseDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public StreamingAccessStatus? AccessStatus { get; set; }

        public int? PaymentId { get; set; }
    }
}
