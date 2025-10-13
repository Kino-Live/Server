using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Halls
{
    public class HallCreateDTO
    {
        [Required]
        [StringLength(256, MinimumLength = 2, ErrorMessage = "Hall name must be between 2 and 256 characters")]
        public string HallName { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Row count must be greater than 0")]
        public int RowCount { get; set; }

        [Required]
        public HallAvailability HallAvailability { get; set; }

        [Required]
        public int CinemaId { get; set; }
    }
}
