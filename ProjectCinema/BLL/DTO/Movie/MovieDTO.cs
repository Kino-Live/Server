using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Movie
{
    public class MovieDTO
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; } = null!;
        public int DurationInMinutes { get; set; }
        public int AgeRestriction { get; set; }
    }
}
