using AutoMapper;
using FlightBooking.Service.Data;
using FlightBooking.Service.Data.DTO;
using FlightBooking.Service.Data.Models;
using FlightBooking.Service.Data.Repository;
using FlightBooking.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Service.Services
{
    ///<inheritdoc />
    public class FlightService : IFlightService
    {
        private readonly IGenericRepository<FlightInformation> _flightRepo;
        private readonly IMapper _mapper;

        public FlightService(IGenericRepository<FlightInformation> flightRepo, IMapper mapper)
        {
            _mapper = mapper;
            _flightRepo = flightRepo;
        }

        public async Task<ServiceResponse<FlightInformationDTO?>> GetFlightInformationAsync(string flightNumber)
        {
            FlightInformation? flight = await _flightRepo.Query()
                .FirstOrDefaultAsync(x => x.FlightNumber == flightNumber);

            if (flight == null)
            {
                return new ServiceResponse<FlightInformationDTO?>(null, InternalCode.EntityNotFound, "flight not found");
            }

            FlightInformationDTO flightDto = _mapper.Map<FlightInformation, FlightInformationDTO>(flight);

            return new ServiceResponse<FlightInformationDTO?>(flightDto, InternalCode.Success);
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