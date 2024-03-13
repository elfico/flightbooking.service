﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using FlightBooking.Service.Data;
using FlightBooking.Service.Data.DTO;
using FlightBooking.Service.Data.Models;
using FlightBooking.Service.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Service.Services
{
    public class ReservedSeatService
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

        public async Task<ServiceResponse<string>> ReserveSeatAsync(string bookingNumber, string seatId)
        {
            //check if booking is valid
            Booking? booking = await _bookingRepo.Query()
               .Include(x => x.FlightInformation)
               .FirstOrDefaultAsync(x => x.BookingNumber == bookingNumber);

            if (booking == null)
            {
                return new ServiceResponse<string>(string.Empty, InternalCode.EntityNotFound, "Booking not found for the supplied booking number");
            }

            //check if seat is available
            bool isSeatAvailable = _seatRepo.Query()
                .Include(x => x.FlightInformation)
                .Any(x => x.FlightNumber == booking.FlightInformation.FlightNumber
                    && x.SeatId == seatId);

            if (!isSeatAvailable)
            {
                return new ServiceResponse<string>(string.Empty, InternalCode.Unprocessable, "The selected seat is not available for selection");
            }

            //reserve the seat
            int result = await _seatRepo.Query()
                .Where(x => x.BookingNumber == bookingNumber && x.SeatId == seatId)
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
