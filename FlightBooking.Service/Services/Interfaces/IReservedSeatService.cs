using FlightBooking.Service.Data.DTO;

namespace FlightBooking.Service.Services.Interfaces
{
    public interface IReservedSeatService
    {
        /// <summary>
        /// Gets all available seats for a flight
        /// </summary>
        /// <param name="flightNumber"></param>
        /// <returns></returns>
        ServiceResponse<IEnumerable<ReservedSeatDTO>> GetAvailableSeatsByFlightNumber(string flightNumber);

        /// <summary>
        /// Creates a seat reservation for a valid booking
        /// </summary>
        /// <param name="requestDTO"></param>
        /// <returns></returns>
        Task<ServiceResponse<string>> ReserveSeatAsync(ReservedSeatRequestDTO requestDTO);


        /// <summary>
        /// Generates seat numbers for a flight based on flight capacity
        /// </summary>
        /// <param name="flightNumber"></param>
        /// <param name="flightCapacity"></param>
        /// <returns></returns>
        Task<ServiceResponse<string>> GenerateSeatNumbersAsync(string flightNumber, int flightCapacity);
    }
}