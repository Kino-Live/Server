using ProjectCinema.BLL.DTO.Common;
using ProjectCinema.BLL.DTO.Movie;
using ProjectCinema.Entities;
using ProjectCinema.Enums;

namespace ProjectCinema.BLL.Interfaces
{
    public interface IMovieService : IGenericService<MovieDTO, Movie>
    {
        Task<MovieDetailsDTO> GetMovieDetailsAsync(int id);
        Task<IEnumerable<MovieDTO>> GetMoviesByStatusAsync(StatusOfMovie movieStatus); 
        Task<MovieDTO> CreateAsync (MovieCreateDTO movieDTO);
        Task<MovieDTO> UpdateAsync (int id, MovieUpdateDTO movieDTO);
        Task<PagedResult<MovieListItemDTO>> GetFilteredMovieAsync(MovieFilterRequestDTO filter, CancellationToken ct);
    }
}
