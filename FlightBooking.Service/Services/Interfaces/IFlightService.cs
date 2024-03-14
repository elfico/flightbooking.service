using FlightBooking.Service.Data.DTO;

namespace FlightBooking.Service.Services.Interfaces
{
    public interface IFlightService
    {
        ServiceResponse<IEnumerable<FlightInformationDTO>> GetFlightInformationAsync(string flightNumber);

        Task<ServiceResponse<string>> UpdateFlightCapacityAsync(string flightNumber, int bookedSeats);
    }
}