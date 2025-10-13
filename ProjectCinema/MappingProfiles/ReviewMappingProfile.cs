using AutoMapper;
using ProjectCinema.BLL.DTO.Review;
using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.BLL.DTO.Movie;
using ProjectCinema.Entities;

namespace ProjectCinema.MappingProfiles
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            // Create automapper for general review info
            CreateMap<Review, ReviewDTO>();

            // Create automapper for details review info
            CreateMap<Review, ReviewDetailsDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Movie, opt => opt.MapFrom(src => src.Movie));

            // Create automapper for creation review
            CreateMap<ReviewCreateDTO, Review>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.MovieId, opt => opt.MapFrom(src => src.MovieId));

            // Create automapper for updating review
            CreateMap<ReviewUpdateDTO, Review>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
