using ProjectCinema.BLL.DTO.ShowTime;
using ProjectCinema.Entities;
using ProjectCinema.Enums;

namespace ProjectCinema.BLL.Interfaces
{
    public interface IShowTimeService : IGenericService<ShowTimeDTO, ShowTime>
    {
        Task<ShowTimeDetailsDTO> GetShowTimeDetailsAsync(int showTimeId);
        Task<ShowTimeDTO> CreateShowTimeAsync(ShowTimeCreateDTO showTimeCreateDTO);
        Task<ShowTimeDTO> UpdateShowTimeAsync(ShowTimeUpdateDTO showTimeUpdateDTO, int showTimeId);
        Task<IEnumerable<ShowTimeDTO>> GetShowTimesByMovieScreeningIdAsync(int movieScreeningId);
        Task<IEnumerable<ShowTimeDTO>> GetShowTimesByHallIdAsync(int hallId);
        Task<IEnumerable<ShowTimeDTO>> GetShowTimesByStatus(ShowTimeStatus showTimeStatus);
        Task<IEnumerable<ShowTimeDTO>> GetAvailiableShowTimesByMovieId(int movieId);
        Task<AvailableDatesDTO> GetAvailableDatesByMovieIdAsync(int movieId);
        Task<ShowTimesByDateResponseDTO> GetShowTimesByMovieIdAndDateAsync(int movieId, DateOnly date);
    }
}
