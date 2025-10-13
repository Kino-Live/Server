using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata;
using ProjectCinema.BLL.DTO.Halls;
using ProjectCinema.BLL.DTO.Seat;
using ProjectCinema.BLL.DTO.ShowTime;
using ProjectCinema.BLL.DTO.Ticket;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.Entities;
using ProjectCinema.Enums;
using ProjectCinema.Repositories.Interfaces;

namespace ProjectCinema.BLL.Services
{
    public class SeatService : GenericService<SeatDTO, Seat>, ISeatService
    {
        private readonly IMapper _mapper;
        private readonly ISeatRepository _seatRepository;
        private readonly ITicketService _ticketService;
        private readonly IShowTimeService _showTimeService;
        private readonly IRowService _rowService;
        private readonly IHallService _hallService;
        public SeatService(ISeatRepository seatRepository,
                           IMapper mapper,
                           ITicketService ticketService,
                           IHallService hallService,
                           IShowTimeService showTimeService,
                           IRowService rowService)
                           : base(seatRepository, mapper)
        {

            _mapper = mapper;
            _seatRepository = seatRepository;
            _ticketService = ticketService;
            _showTimeService = showTimeService;
            _rowService = rowService;
            _hallService = hallService;
        }

        public async Task<SeatDTO> CreateSeatAsync(SeatCreateDTO seatCreateDTO)
        { 

            var seat = _mapper.Map<Seat>(seatCreateDTO);
            seat.CreatedAt = DateTime.Now;
            seat.SeatAvailability = SeatAvailability.Available;
            await _seatRepository.AddAsync(seat);
            await _seatRepository.SaveAsync();

            return _mapper.Map<SeatDTO>(seat);

        }

        public async Task<SeatDetailsDTO> GetSeatDetailsAsync(int seatId)
        {

            if( await _seatRepository.GetByIdAsync(seatId) == null )
            {
                throw new KeyNotFoundException($"Seat with id equal {seatId} does not exists");
            }
            
            Seat seat = await _seatRepository.GetByIdAsync(seatId);
            SeatDetailsDTO seatDTO = _mapper.Map<SeatDetailsDTO>(seat);
            IEnumerable<TicketDTO> ticket = await _ticketService.GetTicketsBySeatIdAsync(seatId);
            _mapper.Map<List<TicketDTO>>(ticket);

            return seatDTO;

        }

        public async Task<IEnumerable<SeatDTO>> GetSeatsByRowIdAsync(int rowId)
        {
            if (await _rowService.GetByIdAsync(rowId) == null)
            {
                throw new KeyNotFoundException($"Hall with id equal {rowId} does not exists");
            }

            IEnumerable<Seat>? seats = await _seatRepository.GetSeatsByRowIdAsync(rowId);

            return _mapper.Map<IEnumerable<SeatDTO>>(seats);

        }

        public Task<IEnumerable<SeatDTO>> GetSeatsByShowTimeId(int showTimeId, SeatAvailability? seatAvailability = null)
        {
            throw new NotImplementedException();
        }

        //public async Task<IEnumerable<SeatDTO>> GetSeatsByShowTimeId(int showTimeId, SeatAvailability? seatAvailability = null)
        //{

        //    ShowTimeDetailsDTO showTime = await _showTimeService.GetShowTimeDetailsAsync(showTimeId);

        //    IEnumerable<HallDetailsDTO> hallDetailsDTOs = await _hallService.G


        //    if (showTime == null)
        //    {
        //        throw new Exception($"ShowTime with id equal {showTimeId} does not exist");
        //    }

        //}

        public async Task<SeatDTO> UpdateSeatAsync(SeatUpdateDTO seatUpdateDTO, int seatId)
        {

            Seat seat = await _seatRepository.GetByIdAsync(seatId);
            _mapper.Map(seatUpdateDTO, seat);

            await _seatRepository.UpdateAsync(seat);
            await _seatRepository.SaveAsync();

            return _mapper.Map<SeatDTO>(seat);

        }
    }
}
