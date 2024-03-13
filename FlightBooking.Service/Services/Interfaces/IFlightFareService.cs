using FlightBooking.Service.Data.DTO;

namespace FlightBooking.Service.Services.Interfaces
{
    public interface IFlightFareService
    {
        ServiceResponse<IEnumerable<FlightFareDTO>?> GetFaresByFlightNumber(string flightNumber);
        Task<ServiceResponse<string>> UpdateFlightFareCapacityAsync(int fareId);
    }
}