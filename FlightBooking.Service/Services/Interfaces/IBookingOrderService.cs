using FlightBooking.Service.Data.DTO;

namespace FlightBooking.Service.Services.Interfaces
{
    public interface IBookingOrderService
    {
        /// <summary>
        /// Create a new order with a list of booking. Accepts one-way, two and multiple bookings per order
        /// </summary>
        /// <param name="order"></param>
        /// <returns>Returns an object containing payment reference and order number</returns>
        Task<ServiceResponse<BookingResponseDTO?>> CreateBookingOrderAsync(BookingOrderDTO order);

        /// <summary>
        /// Creates a Stripe payment link using the order number to search for the particular order
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns>Returns an object containing payment reference and order number</returns>
        Task<ServiceResponse<BookingResponseDTO?>> GetCheckoutUrlAsync(string orderNumber);

        Task<ServiceResponse<string>> UpdateBookingOrderAsync();
    }
}