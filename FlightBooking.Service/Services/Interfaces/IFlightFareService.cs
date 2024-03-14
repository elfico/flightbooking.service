using FlightBooking.Service.Data.DTO;

namespace FlightBooking.Service.Services.Interfaces
{
    public interface IFlightFareService
    {
        /// <summary>
        /// Get all the fares for a flight using flight number
        /// </summary>
        /// <param name="flightNumber"></param>
        /// <returns></returns>
        ServiceResponse<IEnumerable<FlightFareDTO>?> GetFaresByFlightNumber(string flightNumber);

        /// <summary>
        /// Update the flight capacity
        /// </summary>
        /// <param name="fareId"></param>
        /// <returns></returns>
        Task<ServiceResponse<string>> UpdateFlightFareCapacityAsync(int fareId);
    }
}