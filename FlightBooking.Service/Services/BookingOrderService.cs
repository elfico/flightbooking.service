using FlightBooking.Service.Data;

namespace FlightBooking.Service.Services
{
    public class BookingOrderService
    {
        public BookingOrderService() { }

        public ServiceResponse<string> CreateBookingOrder()
        {
            return new ServiceResponse<string>(string.Empty, InternalCode.Success);
        }


    }
}
