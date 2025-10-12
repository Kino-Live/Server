using AutoMapper;
using ProjectCinema.BLL.DTO.Booking;
using ProjectCinema.Entities;

namespace ProjectCinema.MappingProfiles
{
    public class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            // Create automapper for General booking info
            CreateMap<Booking, BookingDTO>();

            //Create automapper for details booking info
            CreateMap<Booking, BookingDetailsDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment))
                .ForMember(dest => dest.Promocode, opt => opt.MapFrom(src => src.Promocode))
                .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets));

            //Create automapper for creation booking
            CreateMap<BookingCreateDTO, Booking>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.PaymentId))
                .ForMember(dest => dest.PromocodeId, opt => opt.MapFrom(src => src.PromocodeId));

            //Create automapper for updating booking
            CreateMap<BookingUpdateDTO, Booking>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); 
        }
    }
}
