using AutoMapper;
using AutoMapper.QueryableExtensions;
using FlightBooking.Service.Data;
using FlightBooking.Service.Data.DTO;
using FlightBooking.Service.Data.Models;
using FlightBooking.Service.Data.Repository;
using FlightBooking.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Service.Services
{
    public class ReservedSeatService : IReservedSeatService
    {
        private readonly IGenericRepository<ReservedSeat> _seatRepo;
        private readonly IGenericRepository<Booking> _bookingRepo;
        private readonly IMapper _mapper;
        public ReservedSeatService(IGenericRepository<ReservedSeat> seatRepository, IGenericRepository<Booking> bookingRepo,
            IMapper mapper)
        {
            _seatRepo = seatRepository;
            _bookingRepo = bookingRepo;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<string>> ReserveSeatAsync(ReservedSeatRequestDTO requestDTO)
        {

            if (requestDTO == null)
            {
                return new ServiceResponse<string>(string.Empty, InternalCode.InvalidParam);
            }

            //check if booking is valid
            Booking? booking = await _bookingRepo.Query()
               .Include(x => x.FlightInformation)
               .FirstOrDefaultAsync(x => x.BookingNumber == requestDTO.BookingNumber);

            if (booking == null)
            {
                return new ServiceResponse<string>(string.Empty, InternalCode.EntityNotFound, "Booking not found for the supplied booking number");
            }

            //check if seat is available
            bool isSeatAvailable = _seatRepo.Query()
                .Include(x => x.FlightInformation)
                .Any(x => x.FlightNumber == booking.FlightInformation.FlightNumber
                    && x.SeatNumber == requestDTO.SeatId);

            if (!isSeatAvailable)
            {
                return new ServiceResponse<string>(string.Empty, InternalCode.Unprocessable, "The selected seat is not available for selection");
            }

            //reserve the seat
            int result = await _seatRepo.Query()
                .Where(x => x.BookingNumber == requestDTO.BookingNumber && x.SeatNumber == requestDTO.SeatId)
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.IsReserved, true));

            return new ServiceResponse<string>(string.Empty, (InternalCode)result);
        }

        public ServiceResponse<IEnumerable<ReservedSeatDTO>> GetAvailableSeatsByFlightNumber(string flightNumber)
        {
            List<ReservedSeatDTO> seats = _seatRepo.Query()
                .Where(x => x.FlightNumber == flightNumber && !x.IsReserved)
                .ProjectTo<ReservedSeatDTO>(_mapper.ConfigurationProvider)
                .ToList();

            return new ServiceResponse<IEnumerable<ReservedSeatDTO>>(seats, InternalCode.Success);
        }
    }
}
