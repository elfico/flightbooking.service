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
    public class FlightFareService : IFlightFareService
    {
        private readonly IGenericRepository<FlightFare> _fareRepository;
        private readonly IMapper _mapper;
        public FlightFareService(IMapper mapper, IGenericRepository<FlightFare> fareRepository)
        {
            _mapper = mapper;
            _fareRepository = fareRepository;
        }

        public ServiceResponse<IEnumerable<FlightFareDTO>?> GetFaresByFlightNumber(string flightNumber)
        {
            var fares = _fareRepository.Query()
                .Include(x => x.FlightInformation)
                .Where(x => x.FlightInformation.FlightNumber == flightNumber)
                .ProjectTo<FlightFareDTO>(_mapper.ConfigurationProvider)
                .ToList();

            //TODO: Check if Flight doesn't exist?

            return new ServiceResponse<IEnumerable<FlightFareDTO>?>(fares, InternalCode.Success);
        }

        public async Task<ServiceResponse<string>> UpdateFlightFareCapacityAsync(int fareId)
        {
            int result = await _fareRepository.Query()
                .Where(x => x.Id == fareId)
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.SeatReserved, y => y.SeatReserved + 1));

            return new ServiceResponse<string>(string.Empty, (InternalCode)result);
        }
    }
}
