using AutoMapper;
using ProjectCinema.BLL.DTO.Notification;
using ProjectCinema.BLL.DTO.Booking;
using ProjectCinema.Entities;

namespace ProjectCinema.MappingProfiles
{
    public class NotificationMappingProfile : Profile
    {
        public NotificationMappingProfile()
        {
            // Create automapper for general notification info
            CreateMap<Notification, NotificationDTO>();

            // Create automapper for details notification info
            CreateMap<Notification, NotificationDetailsDTO>()
                .ForMember(dest => dest.Booking, opt => opt.MapFrom(src => src.Booking));

            // Create automapper for creation notification
            CreateMap<NotificationCreateDTO, Notification>()
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId));

            // Create automapper for updating notification
            CreateMap<NotificationUpdateDTO, Notification>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
