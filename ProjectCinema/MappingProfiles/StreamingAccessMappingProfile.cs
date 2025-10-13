using AutoMapper;
using ProjectCinema.BLL.DTO.StreamingAccess;
using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.BLL.DTO.Movie;
using ProjectCinema.BLL.DTO.Payment;
using ProjectCinema.Entities;

namespace ProjectCinema.MappingProfiles
{
    public class StreamingAccessMappingProfile : Profile
    {
        public StreamingAccessMappingProfile()
        {
            // Create automapper for general streaming access info
            CreateMap<StreamingAccess, StreamingAccessDTO>();

            // Create automapper for details streaming access info
            CreateMap<StreamingAccess, StreamingAccessDetailsDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Movie, opt => opt.MapFrom(src => src.Movie))
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment));

            // Create automapper for creation streaming access
            CreateMap<StreamingAccessCreateDTO, StreamingAccess>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.MovieId, opt => opt.MapFrom(src => src.MovieId));

            // Create automapper for updating streaming access
            CreateMap<StreamingAccessUpdateDTO, StreamingAccess>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
