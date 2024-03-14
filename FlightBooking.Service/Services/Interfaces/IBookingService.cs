using FlightBooking.Service.Data.DTO;

namespace FlightBooking.Service.Services.Interfaces
{
    public interface IBookingService
    {
        Task<ServiceResponse<BookingDTO?>> GetBookingByBookingId(int bookingId);

        Task<ServiceResponse<BookingDTO?>> GetBookingByBookingNumberAsync(string bookingNumber);

        ServiceResponse<IEnumerable<BookingDTO>?> GetBookingsByEmail(string email);

        ServiceResponse<IEnumerable<BookingDTO>?> GetBookingsByOrderNumber(string orderNumber);
    }
}