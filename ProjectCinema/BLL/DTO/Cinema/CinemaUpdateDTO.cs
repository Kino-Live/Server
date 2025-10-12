using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Cinema
{
    public class CinemaUpdateDTO
    {
        [StringLength(256, MinimumLength = 2, ErrorMessage = "Cinema name must be between 2 and 256 characters")]
        public string? CinemaName { get; set; }

        [StringLength(256, MinimumLength = 2, ErrorMessage = "Address must be between 2 and 256 characters")]
        public string? Adress { get; set; }

        [StringLength(500, MinimumLength = 1, ErrorMessage = "Map location must be between 1 and 500 characters")]
        public string? MapLocation { get; set; }
    }
}
