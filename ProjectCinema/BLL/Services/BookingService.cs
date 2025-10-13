using AutoMapper;
using ProjectCinema.BLL.DTO.Booking;
using ProjectCinema.BLL.DTO.Payment;
using ProjectCinema.BLL.DTO.Promocode;
using ProjectCinema.BLL.DTO.Ticket;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.Entities;
using ProjectCinema.Enums;
using ProjectCinema.Repositories.Classes;
using ProjectCinema.Repositories.Interfaces;

namespace ProjectCinema.BLL.Services
{
    public class BookingService : GenericService<BookingDTO, Booking>, IBookingService
    {
        private readonly IMapper _mapper;
        private IBookingRepository _bookingRepository;
        private ITicketService _ticketService;
        private IPaymentService _paymentService;
        private IUserService _userService;
        private IPromocodeService _promocodeService;
        public BookingService(IBookingRepository bookingRepository, 
                              IMapper mapper, 
                              ITicketService ticketService,
                              IPaymentService paymentService,
                              IUserService userService,
                              IPromocodeService promocodeService
                              ) 
                              : base(bookingRepository, mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            _ticketService = ticketService;
            _paymentService = paymentService;
            _userService = userService;
            _promocodeService = promocodeService;
        }

        public async Task<BookingDTO> CreateBookingAsync(BookingCreateDTO bookingCreateDTO)
        {

            Booking booking = _mapper.Map<Booking>(bookingCreateDTO);
            booking.BookingStatus = BookingStatus.Pending;
            booking.CreatedAt = DateTime.Now;

            IEnumerable<TicketDTO> ticketsDTO = await _ticketService.GetTicketsByBookingIdAsync(booking.BookingId);
            Decimal totalPrice = ticketsDTO.Sum(t => t.PriceAtPurchase);
            booking.TotalPrice = totalPrice;

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveAsync();

            return _mapper.Map<BookingDTO>(booking);

        }

        public async Task<BookingDetailsDTO> GetBookingDetailsByIdAsync(int bookingId)
        {

            Booking booking = await _bookingRepository.GetByIdAsync(bookingId);

            if(booking == null)
            {
                throw new InvalidOperationException($"Booking with id equals {bookingId} does not exists");
            }

            IEnumerable<TicketDTO> ticketsDTO = await _ticketService.GetTicketsByBookingIdAsync(bookingId);


            PaymentDTO paymentDTO = null!;

            if (booking.PaymentId.HasValue)
            {
                paymentDTO = await _paymentService.GetByIdAsync(booking.PaymentId.Value);
            }


            BookingDetailsDTO bookingDetailsDTO = _mapper.Map<BookingDetailsDTO>(booking);
            bookingDetailsDTO.Tickets = ticketsDTO.ToList();
            bookingDetailsDTO.Payment = paymentDTO;

            return bookingDetailsDTO;

        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByPromocodeIdAsync(int promocodeId)
        {

            if(await _promocodeService.GetByIdAsync(promocodeId) == null)
            {
                throw new InvalidOperationException($"Promocode with id equals {promocodeId} does not exists");
            }
            
            IEnumerable<Booking> booking = await _bookingRepository.GetBookingsByPromocodeIdAsync(promocodeId);

            return _mapper.Map<List<BookingDTO>>(booking);
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByUserIdAsync(int userId)
        {

            if(await _userService.GetByIdAsync(userId) == null)
            {
                throw new InvalidOperationException($"User with id equals {userId} does not exists");
            }

            IEnumerable<Booking> booking = await _bookingRepository.GetBookingsByUserIdAsync(userId);

            return _mapper.Map<List<BookingDTO>>(booking);
        }

        public async Task<BookingDTO> UpdateBookingAsync(BookingUpdateDTO bookingUpdateDTO, int bookingId)
        {
            if(await _bookingRepository.GetByIdAsync(bookingId) == null)
            {
                throw new InvalidOperationException($"Booking with id equals {bookingId} does not exists");
            }

            Booking booking = await _bookingRepository.GetByIdAsync(bookingId);
            _mapper.Map(bookingUpdateDTO, booking);

            await _bookingRepository.UpdateAsync(booking);
            await _bookingRepository.SaveAsync();

            return _mapper.Map<BookingDTO>(booking);

        }
    }
}
