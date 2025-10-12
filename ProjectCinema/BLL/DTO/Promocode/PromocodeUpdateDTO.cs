using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Promocode
{
    public class PromocodeUpdateDTO
    {
        [StringLength(50, MinimumLength = 3)]
        public string? UniqueCode { get; set; }

        [Range(1, 100, ErrorMessage = "Promocode amount must be between 1 and 100")]
        public decimal? PromocodeAmount { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public bool? IsActive { get; set; }
    }
}
