using Microsoft.AspNetCore.Mvc;
using ProjectCinema.BLL.DTO.Booking;
using ProjectCinema.BLL.Interfaces;

namespace ProjectCinema.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("all-bookings")]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookingsAsync()
        {
            IEnumerable<BookingDTO> bookingDTOs = await _bookingService.GetAllAsync();

            return Ok(bookingDTOs);
        }

        [HttpGet("by-promocode/{Id}")]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookingsByPromocodeIdAsync([FromRoute(Name = "Id")] int promocodeId)
        {
            IEnumerable<BookingDTO> bookingDTOs = await _bookingService.GetBookingsByPromocodeIdAsync(promocodeId);

            return Ok(bookingDTOs);
        }

        [HttpGet("by-user/{Id}")]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookingsByUserIdAsync([FromRoute(Name = "Id")] int userId)
        {
            IEnumerable<BookingDTO> bookingDTOs = await _bookingService.GetBookingsByUserIdAsync(userId);

            return Ok(bookingDTOs);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<BookingDTO>> GetBookingByIdAsync([FromRoute(Name = "Id")] int bookingId)
        {
            BookingDTO bookingDTO = await _bookingService.GetByIdAsync(bookingId);

            return Ok(bookingDTO);
        }

        [HttpGet("details/{Id}")]
        public async Task<ActionResult<BookingDetailsDTO>> GetBookingDetailsByIdAsync([FromRoute(Name = "Id")] int bookingId)
        {
            BookingDetailsDTO bookingDTO = await _bookingService.GetBookingDetailsByIdAsync(bookingId);

            return Ok(bookingDTO);
        }

        [HttpPost("create")]
        public async Task<ActionResult<BookingDTO>> CreateBookingAsync([FromBody] BookingCreateDTO bookingCreateDTO)
        {
            BookingDTO bookingDTO = await _bookingService.CreateBookingAsync(bookingCreateDTO);

            return Ok(bookingDTO);
        }

        [HttpPatch("update")]
        public async Task<ActionResult<BookingDTO>> UpdateBookingAsync([FromBody] BookingUpdateDTO bookingUpdateDTO, [FromRoute(Name = "Id")] int bookingId)
        {
            BookingDTO bookingDTO = await _bookingService.UpdateBookingAsync(bookingUpdateDTO, bookingId);

            return Ok(bookingDTO);
        }
    }
}
