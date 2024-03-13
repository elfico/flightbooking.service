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
    public class FlightService : IFlightService
    {
        private readonly IGenericRepository<FlightInformation> _flightRepo;
        private readonly IMapper _mapper;
        public FlightService(IGenericRepository<FlightInformation> flightRepo, IMapper mapper)
        {
            _mapper = mapper;
            _flightRepo = flightRepo;
        }

        public ServiceResponse<IEnumerable<FlightInformationDTO>> GetFlightInformationAsync(string flightNumber)
        {
            var flights = _flightRepo.Query()
                .Where(x => x.FlightNumber == flightNumber)
                .ProjectTo<FlightInformationDTO>(_mapper.ConfigurationProvider)
                .ToList();

            return new ServiceResponse<IEnumerable<FlightInformationDTO>>(flights, InternalCode.Success);
        }

        public async Task<ServiceResponse<string>> UpdateFlightCapacityAsync(string flightNumber, int bookedSeats)
        {
            int result = await _flightRepo.Query()
                .Where(x => x.FlightNumber == flightNumber)
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.SeatReserved, y => y.SeatReserved + bookedSeats));

            return new ServiceResponse<string>(string.Empty, (InternalCode)result);
        }
    }
}
