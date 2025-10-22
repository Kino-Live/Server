using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectCinema.BLL.DTO.Cinema;
using ProjectCinema.BLL.DTO.Movie;
using ProjectCinema.BLL.DTO.MovieScreening;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.BLL.Interfaces.IMovieScreeningServices;
using ProjectCinema.Data;
using ProjectCinema.Entities;
using ProjectCinema.Enums;
using ProjectCinema.Repositories.Classes;
using ProjectCinema.Repositories.Interfaces;

namespace ProjectCinema.BLL.Services
{
    public class MovieScreeningService : GenericService<MovieScreeningDTO, MovieScreening>, 
                                         IMovieScreeningCrudService, 
                                         IMovieScreeningQueryService, 
                                         IMovieScreeningValidationService
    {
        private readonly IMovieScreeningRepository _movieScreeningRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        //private readonly ICinemaService _cinemaService;
        //private readonly IMovieService _movieService;

        private readonly AplicationDBContext _context;

        public MovieScreeningService(
                              IMovieScreeningRepository movieScreeningRepository, 
                              IMapper mapper,
                              IMovieRepository movieRepository,
                              AplicationDBContext context)
                              : base(movieScreeningRepository, mapper)
        {

            _mapper = mapper;
            _movieScreeningRepository = movieScreeningRepository;
            //_cinemaService = cinemaService;
            //_movieService = movieService;
            _movieRepository = movieRepository;
            _context = context;
        }
        public async Task<MovieScreeningDTO> CreateAsync(MovieScreeningCreateDTO screeningDTO)
        {

            MovieScreening movieScreening = _mapper.Map<MovieScreening>(screeningDTO);
            movieScreening.CreatedAt = DateTime.Now;
            movieScreening.MovieScreeningRelevance = MovieScreeningRelevance.Relevant;

            await _movieScreeningRepository.AddAsync(movieScreening);
            await _movieScreeningRepository.SaveAsync();

            return _mapper.Map<MovieScreeningDTO>(movieScreening);
        }

        public async Task<IEnumerable<MovieScreeningDTO>> GetMovieScreeningsByRelevanceAsync(MovieScreeningRelevance relevance)
        {
            if (!Enum.IsDefined(typeof(MovieScreeningRelevance), relevance))
            {
                throw new ArgumentException($"Invalid relevance value: {relevance}", nameof(relevance));
            }

            IEnumerable<MovieScreening> movieScreenings = await _movieScreeningRepository.GetMovieScreeningsByRelevanceAsync(relevance);

            return _mapper.Map<List<MovieScreeningDTO>>(movieScreenings);

        }

        public async Task<IEnumerable<MovieScreeningDTO>> GetMovieSreeningsByMovieIdAsync(int movieId)
        {
            if( await _movieRepository.GetByIdAsync(movieId) == null)
            {
                throw new KeyNotFoundException($"Movie with id equal {movieId} does not exists");
            }

            IEnumerable<MovieScreening>? movieScreenings = await _movieScreeningRepository.GetMovieSreeningsByMovieIdAsync(movieId);

            return _mapper.Map<IEnumerable<MovieScreeningDTO>>(movieScreenings);

        }

        public async Task<IEnumerable<MovieScreeningDetailsDTO>> GetScreeningsDetailsByMovieIdAsync(int movieId)
        {

            if (await _movieRepository.GetByIdAsync(movieId) == null)
            {
                throw new KeyNotFoundException($"Movie with id equal {movieId} does not exists");
            }

            IEnumerable<MovieScreening> movieScreenings = await _movieScreeningRepository.GetMovieSreeningsByMovieIdAsync(movieId);

            return _mapper.Map<IEnumerable<MovieScreeningDetailsDTO>>(movieScreenings);

        }

        public async Task<MovieScreeningDTO> UpdateAsync(int id, MovieScreeningUpdateDTO screeningDTO)
        {
            if( await _movieScreeningRepository.GetByIdAsync(id) == null)
            {
                throw new KeyNotFoundException($"Movie screening id equal {id} does not exits");
            }

            MovieScreening movieScreening = await  _movieScreeningRepository.GetByIdAsync(id);
            _mapper.Map(screeningDTO, movieScreening);
            await _movieScreeningRepository.UpdateAsync(movieScreening);
            await _movieScreeningRepository.SaveAsync();

            return _mapper.Map<MovieScreeningDTO>(movieScreening);

        }

        public async Task<bool> IsMovieScreeningExistsByCinemaAndMovieAsync(int cinemaId, int movieId)
        {
            throw new NotImplementedException();
            //if( await _cinemaService.GetByIdAsync(cinemaId) == null)
            //{
            //    throw new KeyNotFoundException($"Cinema id equal {cinemaId} does not exists");
            //}

            //if( await _movieRepository.GetByIdAsync(movieId) == null)
            //{
            //    throw new KeyNotFoundException($"Movie id equal {movieId} does not exists");
            //}

            //bool IsExists = await _context.MovieScreenings.AnyAsync(ms => ms.CinemaId == cinemaId && ms.MovieId == movieId);

            //return IsExists;
        }

        public async Task<IEnumerable<MovieScreeningDTO>> GetOverlappingScreeningsAsync(int cinemaId, DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
            //if( await _cinemaService.GetByIdAsync(cinemaId) == null)
            //{
            //    throw new KeyNotFoundException($"Cinema id equal {cinemaId} does not exists");
            //}

            //IEnumerable<MovieScreening> movieScreenings = await _context.MovieScreenings
            //                        .Where(ms => ms.CinemaId == cinemaId &&
            //                        ms.StartDate < endTime &&  
            //                        ms.EndDate > startTime)    
            //                        .ToListAsync();

            //return _mapper.Map<IEnumerable<MovieScreeningDTO>>(movieScreenings);
        }
    }
}
