using ProjectCinema.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Cinema
{
    public class CinemaCreateDTO
    {
        [Required]
        [StringLength(256, MinimumLength = 2, ErrorMessage = "Cinema name must be between 2 and 256 characters")]
        public string CinemaName { get; set; } = null!;

        [Required]
        [StringLength(256, MinimumLength = 2, ErrorMessage = "Address must be between 2 and 256 characters")]
        public string Adress { get; set; } = null!;

        [Required]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Map location must be between 1 and 500 characters")]
        public string MapLocation { get; set; } = null!;
    }
}
