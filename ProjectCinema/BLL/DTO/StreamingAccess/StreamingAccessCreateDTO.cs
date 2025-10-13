using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.StreamingAccess
{
    public class StreamingAccessCreateDTO
    {
        public DateTime PurchaseDate { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public StreamingAccessStatus AccessStatus { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int MovieId { get; set; }
    }
}
