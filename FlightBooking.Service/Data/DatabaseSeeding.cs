using FlightBooking.Service.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Service.Data
{
    public static class DatabaseSeeding
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new FlightBookingContext(serviceProvider.GetRequiredService<DbContextOptions<FlightBookingContext>>()))
            {
                if (context.FlightInformation.Any())
                {
                    return;
                }

                string flightA = Guid.NewGuid().ToString("N")[..4].ToUpper();
                string flightB = Guid.NewGuid().ToString("N")[..4].ToUpper();
                List<FlightFare> flightFaresA = new List<FlightFare>
                {
                    new FlightFare
                    {
                        FareName = "Economy class",
                        FareCode = "Eco-1",
                        Price = 4000,
                        SeatCapacity = 30,
                        SeatReserved = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    },
                    new FlightFare
                    {
                        FareName = "Business class",
                        FareCode = "Biz-1",
                        Price = 4000,
                        SeatCapacity = 30,
                        SeatReserved = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    },
                };

                List<FlightFare> flightFaresB = new List<FlightFare>
                {
                    new FlightFare
                    {
                        FareName = "Economy class",
                        FareCode = "Eco-11",
                        Price = 5000,
                        SeatCapacity = 20,
                        SeatReserved = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    },
                    new FlightFare
                    {
                        FareName = "Business class",
                        FareCode = "Biz-11",
                        Price = 4000,
                        SeatCapacity = 10,
                        SeatReserved = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    },
                };

                List<FlightInformation> flights = new List<FlightInformation>
                {
                    new FlightInformation
                    {
                        SeatCapacity = 60,
                        DepartureDate = DateTime.UtcNow.AddMonths(3),
                        ArrivalDate = DateTime.UtcNow.AddMonths(3).AddHours(4),
                        Airline = "Emirates",
                        SeatReserved = 0,
                        Destination = "London",
                        Origin = "Lagos",
                        FlightNumber = flightA,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        FlightFares = flightFaresA
                    },
                    new FlightInformation
                    {
                        SeatCapacity = 40,
                        DepartureDate = DateTime.UtcNow.AddMonths(3).AddDays(7),
                        ArrivalDate = DateTime.UtcNow.AddMonths(3).AddDays(7).AddHours(4),
                        Airline = "Emirates",
                        SeatReserved = 0,
                        Destination = "Lagos",
                        Origin = "London",
                        FlightNumber = flightB,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        FlightFares = flightFaresB
                    }
                };

                context.FlightInformation.AddRange(flights);

                context.SaveChanges();

                context.Database.EnsureCreated();
            }
        }

    }

}
