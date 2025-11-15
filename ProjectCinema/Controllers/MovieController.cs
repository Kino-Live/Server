using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProjectCinema.BLL.DTO.Common;
using ProjectCinema.BLL.DTO.Movie;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.Enums;
using ProjectCinema.Validations.MovieValidation;

namespace ProjectCinema.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IValidator<MovieCreateDTO> _createValidator;
        private readonly IValidator<MovieUpdateDTO> _updateValidator;
        private readonly IValidator<MovieFilterRequestDTO> _filterValidator;

        public MovieController(IMovieService movieService,
                               IValidator<MovieCreateDTO> createValidator,
                               IValidator<MovieUpdateDTO> updateValidator,
                               IValidator<MovieFilterRequestDTO> filterValidator)
        {
            _movieService = movieService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _filterValidator = filterValidator;
        }
        [HttpGet("allMovies")]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> GetMoviesAsync()
        {
            IEnumerable<MovieDTO> movieDTOs = await _movieService.GetAllAsync();

            return Ok(movieDTOs);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> GetMoviesByStatusAsync([FromQuery] StatusOfMovie status)
        {

            IEnumerable<MovieDTO> movies = await _movieService.GetMoviesByStatusAsync(status);

            return  Ok(movies);
        }

        [HttpGet("details/{Id}")]
        public async Task<ActionResult<MovieDetailsDTO>> GetMovieDetailsByIdAsync([FromRoute(Name = "Id")] int movieId)
        {
            try
            {
                MovieDetailsDTO movieDetailsDTO = await _movieService.GetMovieDetailsAsync(movieId);

                return Ok(movieDetailsDTO);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch(Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }


        [HttpGet("{Id}")]
        public async Task<ActionResult<MovieDTO>> GetMovieByIdAsync([FromRoute(Name = "Id")] int movieId)
        {
            try
            {
                MovieDTO movieDTO = await _movieService.GetByIdAsync(movieId);

                return Ok(movieDTO);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<MovieDTO>> CreateMovieAsync([FromBody] MovieCreateDTO movieCreateDTO)
        {

            var validResult = _createValidator.Validate(movieCreateDTO);

            if (!validResult.IsValid)
            {
                return BadRequest(validResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            MovieDTO movieDTO = await _movieService.CreateAsync(movieCreateDTO);

            return Ok(movieDTO);
        }

        [HttpPatch("update/{Id}")]
        public async Task<ActionResult<MovieDTO>> UpdateMovieAsync([FromRoute(Name = "Id")] int movieId, 
                                                                   [FromBody] MovieUpdateDTO movieUpdateDTO)
        {
            var validResult = _updateValidator.Validate(movieUpdateDTO);

            if (!validResult.IsValid)
            {
                return BadRequest(validResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            try
            {
                MovieDTO movieDTO = await _movieService.GetByIdAsync(movieId);

                return Ok(movieDTO);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<MovieListItemDTO>>> GetFilteredMoviesAsync([FromQuery] MovieFilterRequestDTO filter, CancellationToken ct)
        {
            var validResult = _filterValidator.Validate(filter);

            if (!validResult.IsValid)
            {
                return BadRequest(validResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }
            var result = await _movieService.GetFilteredMovieAsync(filter, ct);
            return Ok(result);
        }
    }
}
