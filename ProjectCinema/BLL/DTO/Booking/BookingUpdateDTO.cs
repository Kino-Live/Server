using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Booking
{
    public class BookingUpdateDTO
    {
        public BookingStatus? BookingStatus { get; set; }
    }
}
