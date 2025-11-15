using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectCinema.BLL.DTO.Common;
using ProjectCinema.BLL.DTO.Movie;
using ProjectCinema.BLL.DTO.MovieScreening;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.BLL.Interfaces.IMovieScreeningServices;
using ProjectCinema.DAL.Models;
using ProjectCinema.Data;
using ProjectCinema.Entities;
using ProjectCinema.Enums;
using ProjectCinema.Repositories.Classes;
using ProjectCinema.Repositories.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.Services
{
    public class MovieService : GenericService<MovieDTO, Movie>, IMovieService
    {

        private readonly IMovieScreeningQueryService _screeningQueryService;
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        public MovieService(IMovieRepository movieRepository, 
                            IMapper mapper,
                            IMovieScreeningQueryService screeningQueryService)
                            :base(movieRepository, mapper)
        {
            _mapper = mapper;
            _movieRepository = movieRepository;
            _screeningQueryService = screeningQueryService;

        }
        public async Task<MovieDTO> CreateAsync(MovieCreateDTO movieDTO)
        {

            Movie movie = _mapper.Map<Movie>(movieDTO);
            movie.Status = StatusOfMovie.Active;
            movie.CreatedAt = DateTime.Now;
            await _movieRepository.AddAsync(movie);
            await _movieRepository.SaveAsync();

            return _mapper.Map<MovieDTO>(movie);
        }

        public async Task<IEnumerable<MovieDTO>> GetMoviesByStatusAsync(StatusOfMovie statusOfMovie)
        {

            IEnumerable<Movie> movies  = await _movieRepository.GetMoviesByStatusAsync(statusOfMovie);

            return _mapper.Map<IEnumerable<MovieDTO>>(movies);

        }

        public async Task<MovieDetailsDTO> GetMovieDetailsAsync(int id)
        {
            Movie movie = await _movieRepository.GetByIdAsync(id);
            
            if (movie == null)
            {
                throw new KeyNotFoundException($"Movie id equal {id} does not exists");
            }
            IEnumerable<MovieScreeningDTO> movieScreenings = await _screeningQueryService.GetMovieSreeningsByMovieIdAsync(movie.MovieId);
            MovieDetailsDTO movieDto = _mapper.Map<MovieDetailsDTO>(movie);
            movieDto.MovieScreenings = movieScreenings.ToList();

            return movieDto;

        }

        public async Task<MovieDTO> UpdateAsync(int id, MovieUpdateDTO movieDTO)
        {
            Movie movie = await _movieRepository.GetByIdAsync(id);
            
            if (movie == null)
            {
                throw new KeyNotFoundException($"Movie id equal {id} does not exists");
            }
            _mapper.Map(movieDTO, movie);
            await _movieRepository.UpdateAsync(movie);
            await _movieRepository.SaveAsync();

            return _mapper.Map<MovieDTO>(movie);

        }

        public async Task<PagedResult<MovieListItemDTO>> GetFilteredMovieAsync(MovieFilterRequestDTO filter, CancellationToken ct)
        {
            var filterParams = new MovieFilterParams
            {
                Genres = filter.Genres,
                YearFrom = filter.YearFrom,
                YearTo = filter.YearTo,
                RatingMin = filter.RatingMin,
                RatingMax = filter.RatingMax
            };

            var query = _movieRepository.BuildFilteredQuery(filterParams);

            var totalCount = await query.CountAsync(ct);

            query = query.OrderByDescending(m => m.ReleaseYear.Year);

            int page = filter.Page < 1 ? 1 : filter.Page;
            int pageSize = filter.PageSize > 100 ? 100 : filter.PageSize;

            var movies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            IEnumerable<MovieListItemDTO> movieDtos = _mapper.Map<IEnumerable<MovieListItemDTO>>(movies);

            var result = new PagedResult<MovieListItemDTO>
            {
                Items = movieDtos,
                Total = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return result;
        }
    }
}
