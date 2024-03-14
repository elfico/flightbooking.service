using FlightBooking.Service.Data.DTO;

namespace FlightBooking.Service.Services.Interfaces
{
    public interface IReservedSeatService
    {
        ServiceResponse<IEnumerable<ReservedSeatDTO>> GetAvailableSeatsByFlightNumber(string flightNumber);
        Task<ServiceResponse<string>> ReserveSeatAsync(ReservedSeatRequestDTO requestDTO);
    }
}