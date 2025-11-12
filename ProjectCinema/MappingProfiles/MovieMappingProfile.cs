using AutoMapper;
using ProjectCinema.BLL.DTO.Movie;
using ProjectCinema.BLL.DTO.Review;
using ProjectCinema.BLL.DTO.StreamingAccess;
using ProjectCinema.Entities;

namespace ProjectCinema.MappingProfiles
{
    public class MovieMappingProfile : Profile
    {
        public MovieMappingProfile()
        {
            // Create automapper for general movie info
            CreateMap<Movie, MovieDTO>();

            //Create automapper for details movie info
            CreateMap<Movie, MovieDetailsDTO>()
                .ForMember(dest => dest.MovieScreenings, opt => opt.MapFrom(src => src.MovieScreenings))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews))
                .ForMember(dest => dest.StreamingAccesses, opt => opt.MapFrom(src => src.StreamingAccesses));

            //Create automapper for creation the movie
            CreateMap<MovieCreateDTO, Movie>();

            //Create automapper for updating the movie
            CreateMap<MovieUpdateDTO, Movie>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Movie, MovieListItemDTO>()
                .ForMember(dest => dest.ReleaseYear, opt => opt.MapFrom(src => src.ReleaseYear.Year));
        }
    }
}
