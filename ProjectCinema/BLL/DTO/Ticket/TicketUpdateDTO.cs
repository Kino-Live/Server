using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Ticket
{
    public class TicketUpdateDTO
    {
        public TicketStatus? TicketStatus { get; set; }

        public Decimal? PriceAtPurchase { get; set; }
    }
}
