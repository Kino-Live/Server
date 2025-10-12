using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Promocode
{
    public class PromocodeCreateDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string UniqueCode { get; set; } = null!;

        [Required]
        [Range(1, 100, ErrorMessage = "Promocode amount must be between 1 and 100")]
        public decimal PromocodeAmount { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
