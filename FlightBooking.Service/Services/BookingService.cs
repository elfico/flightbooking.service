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
    public class BookingService : IBookingService
    {
        private readonly IGenericRepository<Booking> _bookingRepo;
        private readonly IMapper _mapper;
        public BookingService(IGenericRepository<Booking> bookingRepo, IMapper mapper)
        {
            _bookingRepo = bookingRepo;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<BookingDTO?>> GetBookingByBookingNumberAsync(string bookingNumber)
        {
            if (string.IsNullOrWhiteSpace(bookingNumber))
            {
                return new ServiceResponse<BookingDTO?>(null, InternalCode.InvalidParam, "No booking number supplied");
            }

            var booking = await _bookingRepo.Query()
                .Include(x => x.FlightFare)
                .Include(x => x.FlightInformation)
                .FirstOrDefaultAsync(x => x.BookingNumber == bookingNumber);

            if (booking == null)
            {
                return new ServiceResponse<BookingDTO?>(null, InternalCode.EntityNotFound, "No booking with that booking number exists");
            }

            var bookingDTO = _mapper.Map<Booking, BookingDTO>(booking);


            return new ServiceResponse<BookingDTO?>(bookingDTO, InternalCode.Success);
        }

        public async Task<ServiceResponse<BookingDTO?>> GetBookingByBookingId(int bookingId)
        {
            var booking = await _bookingRepo.Query()
                .Include(x => x.FlightFare)
                .Include(x => x.FlightInformation)
                .FirstOrDefaultAsync(x => x.Id == bookingId);

            if (booking == null)
            {
                return new ServiceResponse<BookingDTO?>(null, InternalCode.EntityNotFound, "No booking with that booking ID exists");
            }

            var bookingDTO = _mapper.Map<Booking, BookingDTO>(booking);

            return new ServiceResponse<BookingDTO?>(bookingDTO, InternalCode.Success);
        }

        public ServiceResponse<IEnumerable<BookingDTO>?> GetBookingsByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ServiceResponse<IEnumerable<BookingDTO>?>(null, InternalCode.InvalidParam, "No booking number supplied");
            }

            var bookings = _bookingRepo.Query()
                .Include(x => x.FlightFare)
                .Include(x => x.FlightInformation)
                .Where(x => x.Email == email)
                .ProjectTo<BookingDTO>(_mapper.ConfigurationProvider)
                .ToList();

            return new ServiceResponse<IEnumerable<BookingDTO>?>(bookings, InternalCode.Success);
        }
    }
}
