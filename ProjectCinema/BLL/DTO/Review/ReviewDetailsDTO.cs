using System.ComponentModel.DataAnnotations;
using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.BLL.DTO.Movie;

namespace ProjectCinema.BLL.DTO.Review
{
    public class ReviewDetailsDTO
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }

        public UserDTO? User { get; set; }
        public MovieDTO? Movie { get; set; }
    }
}
