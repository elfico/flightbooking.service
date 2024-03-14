using FlightBooking.Service.Services.Interfaces;

namespace FlightBooking.Service.Services
{
    public static class ServicesModule
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IBookingOrderService, BookingOrderService>();
            services.AddScoped<IReservedSeatService, ReservedSeatService>();
            services.AddScoped<IFlightFareService, FlightFareService>();
            services.AddScoped<IFlightService, FlightService>();
            services.AddScoped<IStripeService, StripeService>();
        }
    }
}