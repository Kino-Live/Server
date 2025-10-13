using ProjectCinema.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Row
{
    public class RowDTO
    {
        public int RowId { get; set; }
        public int RowNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int HallId { get; set; }
    }
}
