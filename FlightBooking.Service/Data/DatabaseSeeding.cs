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

                //Create available seats
                var reservedSeatsA = GenerateSeats(flightA, 60);
                var reservedSeatsB = GenerateSeats(flightB, 60);

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
                        FlightFares = flightFaresA,
                        ReservedSeats = reservedSeatsA,
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
                        FlightFares = flightFaresB,
                        ReservedSeats = reservedSeatsB
                    }
                };

                context.FlightInformation.AddRange(flights);

                context.SaveChanges();

                context.Database.EnsureCreated();
            }
        }

        private static List<ReservedSeat> GenerateSeats(string flightNumber, int flightCapacity)
        {
            //assume seats are in group of 4 Alphabets e.g 1A, 1B, 1C, 1D

            Dictionary<int, string> SeatMaps = new Dictionary<int, string>
            {
                {1, "A" },
                {2, "B" },
                {3, "C" },
                {4, "D" }
            };

            int seatId = 1;
            int seatCount = 1;

            List<string> seatNumbers = new List<string>();

            for (int i = 1; i < flightCapacity + 1; i++)
            {
                if (seatCount > 4)
                {
                    seatId++;
                    seatCount = 1;
                }

                seatNumbers.Add(seatId + SeatMaps[seatCount]);
                seatCount++;
            }

            List<ReservedSeat> reservedSeats = new List<ReservedSeat>();

            foreach (var seatNumber in seatNumbers)
            {
                reservedSeats.Add(new ReservedSeat
                {
                    BookingNumber = null,
                    FlightNumber = flightNumber,
                    IsReserved = false,
                    SeatNumber = seatNumber
                });
            }

            return reservedSeats;
        }
    }
}