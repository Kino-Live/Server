using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Review
{
    public class ReviewDTO
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
    }
}
