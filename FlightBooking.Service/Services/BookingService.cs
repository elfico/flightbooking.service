using FlightBooking.Service.Data;

namespace FlightBooking.Service.Services
{
    public class BookingService
    {
        public BookingService() { }

        public ServiceResponse<string> GetBookingByBookingNumber(string bookingNumber)
        {
            return new ServiceResponse<string>(string.Empty, InternalCode.Success);
        }
    }
}
