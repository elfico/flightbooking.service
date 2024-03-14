using FlightBooking.Service.Data.DTO;

namespace FlightBooking.Service.Services.Interfaces
{
    public interface IFlightService
    {
        /// <summary>
        /// Gets the flight information for a flight
        /// </summary>
        /// <param name="flightNumber"></param>
        /// <returns></returns>
        Task<ServiceResponse<FlightInformationDTO?>> GetFlightInformationAsync(string flightNumber);

        /// <summary>
        /// Updates the flight capacity
        /// </summary>
        /// <param name="flightNumber"></param>
        /// <param name="bookedSeats"></param>
        /// <returns></returns>
        Task<ServiceResponse<string>> UpdateFlightCapacityAsync(string flightNumber, int bookedSeats);
    }
}