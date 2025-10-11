using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProjectCinema.BLL.DTO.ShowTime;
using ProjectCinema.BLL.DTO.Ticket;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.Enums;


namespace ProjectCinema.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowTimeController : ControllerBase
    {
        private readonly IShowTimeService _showTimeService;
        private readonly IValidator<ShowTimeCreateDTO> _showTimeCreateValidator;
        private readonly IValidator<ShowTimeUpdateDTO> _showTimeUpdateValidator;

        public ShowTimeController(IShowTimeService showTimeService, 
                                  IValidator<ShowTimeCreateDTO> showTimeCreateValidator,
                                  IValidator<ShowTimeUpdateDTO> showTimeUpdateValidator)
        {
            _showTimeService = showTimeService;
            _showTimeCreateValidator = showTimeCreateValidator;
            _showTimeUpdateValidator = showTimeUpdateValidator;
        }

        [HttpGet("all-showTimes")]
        public async Task<ActionResult<IEnumerable<ShowTimeDTO>>> GetShowTimesAsync()
        {
            IEnumerable<ShowTimeDTO> showTimesDTOs = await _showTimeService.GetAllAsync();

            return Ok(showTimesDTOs);
        }

        [HttpGet("by-hall/{Id}")]
        public async Task<ActionResult<IEnumerable<ShowTimeDTO>>> GetShowTimesByHallIdAsync([FromRoute(Name = "Id")] int showTimeId)
        {
            try
            {
                IEnumerable<ShowTimeDTO> showTimeDTOs = await _showTimeService.GetShowTimesByHallIdAsync(showTimeId);

                return Ok(showTimeDTOs);
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

        [HttpGet("by-movieScreening/{Id}")]
        public async Task<ActionResult<IEnumerable<ShowTimeDTO>>> GetShowTimesByMovieScreeningIdAsync([FromRoute(Name = "Id")] int movieScreeningId)
        {
            try
            {
                IEnumerable<ShowTimeDTO> showTimeDTOs = await _showTimeService.GetShowTimesByMovieScreeningIdAsync(movieScreeningId);

                return Ok(showTimeDTOs);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch( Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("availiable/by-movie/{Id}")]
        public async Task<ActionResult<IEnumerable<ShowTimeDTO>>> GetAvailiableShowTimesByMovieIdAsync([FromRoute(Name = "Id")] int movieId)
        {
            try
            {
                IEnumerable<ShowTimeDTO> showTimeDTOs = await _showTimeService.GetAvailiableShowTimesByMovieId(movieId);

                return Ok(showTimeDTOs);
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

        [HttpGet("by-status")]
        public async Task<ActionResult<IEnumerable<ShowTimeDTO>>> GetShowTimesByStatusAsync([FromQuery] ShowTimeStatus showTimeStatus)
        {
            IEnumerable<ShowTimeDTO> showTimeDTOs = await _showTimeService.GetShowTimesByStatus(showTimeStatus);

            return Ok(showTimeDTOs);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<ShowTimeDTO>> GetShowTimeByIdAsync([FromRoute(Name = "Id")] int showTimeId)
        {
            try
            {
                ShowTimeDTO showTimeDTO = await _showTimeService.GetByIdAsync(showTimeId);

                return Ok(showTimeDTO);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("details/{Id}")]
        public async Task<ActionResult<ShowTimeDTO>> GetShowTimeDetailsAsync([FromRoute(Name = "Id")] int showTimeId)
        {
            try
            {
                ShowTimeDetailsDTO showTimeDTO = await _showTimeService.GetShowTimeDetailsAsync(showTimeId);

                return Ok(showTimeDTO);
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

        [HttpPost("create")]
        public async Task<ActionResult<ShowTimeDTO>> CreateShowTimeAsync([FromBody] ShowTimeCreateDTO showTimeCreateDTO)
        {
            var validResult = await _showTimeCreateValidator.ValidateAsync(showTimeCreateDTO);

            if(!validResult.IsValid)
            {
                return BadRequest(validResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage,
                }));
            }

            ShowTimeDTO showTimeDTO = await _showTimeService.CreateShowTimeAsync(showTimeCreateDTO);

            return Ok(showTimeDTO);
        }

        [HttpPatch("update")]
        public async Task<ActionResult<ShowTimeDTO>> UpdateShowTimeAsync([FromBody] ShowTimeUpdateDTO showTimeUpdateDTO, [FromRoute(Name = "Id")] int showTimeId)
        {
            var validResult = await _showTimeUpdateValidator.ValidateAsync(showTimeUpdateDTO);

            if(!validResult.IsValid)
            {
                return BadRequest(validResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage,
                }));
            }
            try
            {
                ShowTimeDTO showTimeDTO = await _showTimeService.UpdateShowTimeAsync(showTimeUpdateDTO, showTimeId);

                return Ok(showTimeDTO);
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
    }
}
