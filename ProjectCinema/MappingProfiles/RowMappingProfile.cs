using AutoMapper;
using ProjectCinema.BLL.DTO.Row;
using ProjectCinema.BLL.DTO.Seat;
using ProjectCinema.BLL.DTO.Halls;
using ProjectCinema.Entities;

namespace ProjectCinema.MappingProfiles
{
    public class RowMappingProfile : Profile 
    {
        public RowMappingProfile() 
        {
            // Create automapper for general row info
            CreateMap<Row, RowDTO>();

            // Create automapper for details row info
            CreateMap<Row, RowDetailsDTO>()
                .ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.Seats))
                .ForMember(dest => dest.Hall, opt => opt.MapFrom(src => src.Hall));

            // Create automapper for creation the row
            CreateMap<RowCreateDTO, Row>()
                .ForMember(dest => dest.HallId, opt => opt.MapFrom(src => src.HallId));

            // Create automapper for updating the row
            CreateMap<RowUpdateDTO, Row>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
