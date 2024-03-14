using FlightBooking.Service.Data.DTO;

namespace FlightBooking.Service.Services.Interfaces
{
    public interface IBookingOrderService
    {
        Task<ServiceResponse<BookingResponseDTO?>> CreateBookingOrderAsync(BookingOrderDTO order);
        Task<ServiceResponse<BookingResponseDTO?>> GetCheckoutUrlAsync(string orderReference);
    }
}