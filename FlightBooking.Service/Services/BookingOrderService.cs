using FlightBooking.Service.Data;
using FlightBooking.Service.Data.DTO;
using FlightBooking.Service.Data.Models;
using FlightBooking.Service.Data.Repository;
using Humanizer;
using Stripe;

namespace FlightBooking.Service.Services
{
    public class BookingOrderService
    {
        private readonly IGenericRepository<BookingOrder> _orderRepo;
        private readonly IGenericRepository<FlightInformation> _flightRepo;
        private readonly IGenericRepository<FlightFare> _flightFareRepo;

        public BookingOrderService() { }

        public ServiceResponse<string> CreateBookingOrder(BookingOrderDTO order)
        {

            if (order == null)
            {
                return new ServiceResponse<string>(string.Empty, InternalCode.InvalidParam);
            }

            //TODO: Check if List == 0

            /*
                Checks:
                - Check if all the flights are available
                - Check if the fares have available seats
                - If Checks are not passed, return 422
             */

            bool hasReturnFlight = !string.IsNullOrWhiteSpace(order.ReturnFlightNumber);
            int totalFlights = order.Bookings.Count;

            var initialFlight = CheckAvailableFlight(order.FlightNumber, totalFlights);

            //If flight not valid, return false
            if (initialFlight.FlightInformation == null)
            {
                return new ServiceResponse<string>(string.Empty, InternalCode.Unprocessable, "The selected flight is not valid");
            }

            if (initialFlight.IsBookedToMax)
            {
                return new ServiceResponse<string>(string.Empty, InternalCode.Unprocessable, "The number of flights exceed number of available seats");
            }

            //Check if Return flight available

            if (hasReturnFlight)
            {
                var returnFlight = CheckAvailableFlight(order.ReturnFlightNumber!, totalFlights);

                if (returnFlight.FlightInformation == null)
                {
                    return new ServiceResponse<string>(string.Empty, InternalCode.Unprocessable, "The selected flight is not valid");
                }

                if (returnFlight.IsBookedToMax)
                {
                    return new ServiceResponse<string>(string.Empty, InternalCode.Unprocessable, "The number of flights exceed number of available seats");
                }
            }

            //Check if the selected Fare has seats available
            bool isAnyFareMaxedOut = CheckIfAnyFareIsMaxedOut(order, hasReturnFlight);

            if (isAnyFareMaxedOut)
            {
                return new ServiceResponse<string>(string.Empty, InternalCode.Unprocessable, "The number of flights exceed number of available seats");
            }

            //if all checks passed, lets reduce number of available seats.
            //If Checks are passed, set status as InProgress and reduce available seats to avoid overbooking the flight
            //We hold a seat for N mins max(N is set in config). Use a background job to return UnbookedSeats back into the Pool

            //Create all the bookings

            //Sum all cost as part of Order


            //Generate payment link with the cost

            //

            /*
                If  
             */
            return new ServiceResponse<string>(string.Empty, InternalCode.Success);
        }

        private (FlightInformation? FlightInformation, bool IsBookedToMax) CheckAvailableFlight(string flightNumber, int totalFlights)
        {
            FlightInformation? flightInformation = _flightRepo.Query()
                .FirstOrDefault(x => x.FlightNumber == flightNumber);

            //If flight not valid, return false
            if (flightInformation == null)
            {
                return (flightInformation, false);
            }

            int availableFlightCapacity = flightInformation.SeatCapacity - flightInformation.SeatReserved;

            return (flightInformation, totalFlights > availableFlightCapacity);
        }

        private bool CheckIfAnyFareIsMaxedOut(BookingOrderDTO order, bool hasReturnFlight)
        {
            List<string> fareCodes = new List<string>();
            var initialFares = order.Bookings.Select(x => x.FlightFareCode).ToList();
            fareCodes.AddRange(initialFares);
            if (hasReturnFlight)
            {
                //if the booking as not return flight fare, use the initial flight fare
                foreach (var booking in order.Bookings)
                {
                    if (string.IsNullOrWhiteSpace(booking.ReturnFlightFareCode))
                    {
                        fareCodes.Add(booking.FlightFareCode);
                        continue;
                    }
                    fareCodes.Add(booking.ReturnFlightFareCode!);

                }
            }

            //get all flight fares
            var flightFares = _flightFareRepo.Query()
                .Where(x => fareCodes.Contains(x.FareCode))
                .Select(y => new
                {
                    y.FareCode,
                    AvailableSeats = y.SeatCapacity - y.SeatReserved
                }).ToList();

            var fareGroup = fareCodes.GroupBy(x => x).ToList();

            bool isAnyFareMaxedOut = true; 
            foreach(var group in fareGroup)
            {
                isAnyFareMaxedOut = flightFares.Any(x => x.FareCode == group.Key && group.Count() > x.AvailableSeats);
                if (isAnyFareMaxedOut)
                {
                    break;
                }
            }

            return isAnyFareMaxedOut;
        }
    }
}
