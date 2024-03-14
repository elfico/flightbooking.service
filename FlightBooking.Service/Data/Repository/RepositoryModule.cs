using FlightBooking.Service.Data.Models;

namespace FlightBooking.Service.Data.Repository
{
    public static class RepositoryModule
    {
        public static void AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IGenericRepository<Booking>, GenericRepository<Booking>>();
            services.AddScoped<IGenericRepository<FlightInformation>, GenericRepository<FlightInformation>>();
            services.AddScoped<IGenericRepository<FlightFare>, GenericRepository<FlightFare>>();
            services.AddScoped<IGenericRepository<Payment>, GenericRepository<Payment>>();
            services.AddScoped<IGenericRepository<BookingOrder>, GenericRepository<BookingOrder>>();
            services.AddScoped<IGenericRepository<ReservedSeat>, GenericRepository<ReservedSeat>>();
        }
    }
}