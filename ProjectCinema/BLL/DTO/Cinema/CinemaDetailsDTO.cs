using ProjectCinema.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ProjectCinema.BLL.DTO.MovieScreening;
using ProjectCinema.BLL.DTO.Halls;

namespace ProjectCinema.BLL.DTO.Cinema
{
    public class CinemaDetailsDTO
    {
        public int CinemaId { get; set; }
        public string? CinemaName { get; set; }
        public string? Adress { get; set; }
        public string MapLocation { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<MovieScreeningDTO>? MovieScreenings { get; set; }
        public ICollection<HallDTO>? Halls { get; set; }
    }
}
