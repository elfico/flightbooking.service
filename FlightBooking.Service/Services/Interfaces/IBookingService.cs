using FlightBooking.Service.Data.DTO;

namespace FlightBooking.Service.Services.Interfaces
{
    public interface IBookingService
    {
        /// <summary>
        /// Get booking information using booking ID
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        Task<ServiceResponse<BookingDTO?>> GetBookingByBookingId(int bookingId);

        /// <summary>
        /// Get booking information using booking number
        /// </summary>
        /// <param name="bookingNumber"></param>
        /// <returns></returns>
        Task<ServiceResponse<BookingDTO?>> GetBookingByBookingNumberAsync(string bookingNumber);

        /// <summary>
        /// Get all bookings for an email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        ServiceResponse<IEnumerable<BookingDTO>?> GetBookingsByEmail(string email);

        /// <summary>
        /// Get all booking for an order
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        ServiceResponse<IEnumerable<BookingDTO>?> GetBookingsByOrderNumber(string orderNumber);
    }
}