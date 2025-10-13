using AutoMapper;
using ProjectCinema.BLL.DTO.Payment;
using ProjectCinema.BLL.DTO.StreamingAccess;
using ProjectCinema.Entities;

namespace ProjectCinema.MappingProfiles
{
    public class PaymentMappingProfile : Profile
    {
        public PaymentMappingProfile()
        {
            //Create automapper for general payment info
            CreateMap<Payment, PaymentDTO>();

            //Create automapper for details payment info
            CreateMap<Payment, PaymentDetailsDTO>()
                .ForMember(dest => dest.Booking, opt => opt.MapFrom(src => src.Booking))
                .ForMember(dest => dest.StreamingAccess, opt => opt.MapFrom(src => src.StreamingAccess));

            //Create automapper for creation the payment
            CreateMap<PaymentCreateDTO, Payment>()
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId));

            //Create automapper for updating the payment
            CreateMap<PaymentUpdateDTO, Payment>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
