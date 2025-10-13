using ProjectCinema.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Row
{
    public class RowCreateDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Row number must be greater than 0")]
        public int RowNumber { get; set; }

        [Required]
        public int HallId { get; set; }
    }
}
