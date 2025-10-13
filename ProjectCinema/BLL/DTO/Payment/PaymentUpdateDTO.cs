using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Payment
{
    public class PaymentUpdateDTO
    {
        public PaymentMethod? PaymentMethod { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal? AmountPaid { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
