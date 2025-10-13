using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Ticket
{
    public class TicketUpdateDTO
    {
        public TicketStatus? TicketStatus { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? PriceAtPurchase { get; set; }
    }
}
