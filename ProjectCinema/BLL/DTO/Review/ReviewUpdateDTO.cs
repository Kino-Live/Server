using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Review
{
    public class ReviewUpdateDTO
    {
        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10")]
        public int? Rating { get; set; }

        [StringLength(1028, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 1028 characters")]
        public string? Message { get; set; }
    }
}
