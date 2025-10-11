using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProjectCinema.BLL.DTO.Ticket;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.Enums;

namespace ProjectCinema.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IValidator<TicketCreateDTO> _ticketCreateValidator;
        private readonly IValidator<TicketUpdateDTO> _ticketUpdateValidator;
        public TicketController(ITicketService ticketService, 
                                IValidator<TicketUpdateDTO> ticketUpdateValidator,
                                IValidator<TicketCreateDTO> ticketCreateValidator)
        {
            _ticketService = ticketService;
            _ticketUpdateValidator = ticketUpdateValidator;
            _ticketCreateValidator = ticketCreateValidator;
        }

        [HttpGet("all-tickets")]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetTicketsAsync()
        {
            IEnumerable<TicketDTO> ticketDTOs = await _ticketService.GetAllAsync();

            return Ok(ticketDTOs);
        }

        [HttpGet("by-status")]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetTicketsByTicketStatusAsync([FromQuery] TicketStatus ticketStatus)
        {
            try
            {
                IEnumerable<TicketDTO> ticketDTOs = await _ticketService.GetTicketsByTicketStatusAsync(ticketStatus);

                return Ok(ticketDTOs);
            }
            catch(KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("by-booking/{Id}")]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetTicketsByBookingIdAsync([FromRoute(Name ="Id")] int bookingId)
        {
            try
            {
                IEnumerable<TicketDTO> ticketDTOs = await _ticketService.GetTicketsByBookingIdAsync(bookingId);

                return Ok(ticketDTOs);
            }
            catch(KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("by-seat/{Id}")]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetTicketsBySeatIdAsync([FromRoute(Name = "Id")] int seatId)
        {
            try
            {
                IEnumerable<TicketDTO> ticketDTOs = await _ticketService.GetTicketsBySeatIdAsync(seatId);

                return Ok(ticketDTOs);
            }
            catch(KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("by-showTime/{Id}")]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetTicketsByShowTimeIdAsync([FromRoute(Name = "Id")] int showTimeId)
        {
            try
            {
                IEnumerable<TicketDTO> ticketDTOs = await _ticketService.GetTicketsByShowTimeIdAsync(showTimeId);

                return Ok(ticketDTOs);
            }
            catch(KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<TicketDTO>> GetTicketByIdAsync([FromRoute(Name = "Id")] int ticketId)
        {
            try
            {
                TicketDTO ticketDTO = await _ticketService.GetByIdAsync(ticketId);

                return Ok(ticketDTO);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<TicketDTO>> CreateTicketAsync([FromBody] TicketCreateDTO ticketCreateDTO)
        {
            var validResult = await _ticketCreateValidator.ValidateAsync(ticketCreateDTO);
            if(!validResult.IsValid)
            {
                return BadRequest(validResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage,
                }));
            }

            TicketDTO ticketDTO = await _ticketService.CreateTicketAsync(ticketCreateDTO);  

            return Ok(ticketDTO);
        }

        [HttpPatch("update")]
        public async Task<ActionResult<TicketDTO>> UpdateTicketAsync([FromBody] TicketUpdateDTO ticketUpdateDTO, [FromRoute(Name = "Id")] int ticketId)
        {
            var validResult = await _ticketUpdateValidator.ValidateAsync(ticketUpdateDTO);
            if (!validResult.IsValid)
            {
                return BadRequest(validResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage,
                }));
            }

            TicketDTO ticketDTO = await _ticketService.UpdateTicketAsync(ticketUpdateDTO, ticketId);

            return Ok(ticketDTO);
        }

    }
}
