using FlightBooking.Service.Data;
using FlightBooking.Service.Data.DTO;
using FlightBooking.Service.Data.Models;
using FlightBooking.Service.Data.Repository;
using FlightBooking.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Service.Services
{
    public class BookingOrderService : IBookingOrderService
    {
        private readonly IGenericRepository<BookingOrder> _orderRepo;
        private readonly IGenericRepository<FlightInformation> _flightRepo;
        private readonly IGenericRepository<FlightFare> _flightFareRepo;
        private readonly IStripeService _stripeService;

        private readonly ILogger<BookingOrderService> _logger;

        private List<FlightFare> allFlightFares = new List<FlightFare>(); //declare once so we can use it throughout

        public BookingOrderService(IGenericRepository<BookingOrder> orderRepo, IGenericRepository<FlightInformation> flightRepo,
            IGenericRepository<FlightFare> flightFareRepo, IStripeService stripeService, ILogger<BookingOrderService> logger)
        {
            _orderRepo = orderRepo;
            _flightRepo = flightRepo;
            _flightFareRepo = flightFareRepo;
            _stripeService = stripeService;
            _logger = logger;
        }

        public async Task<ServiceResponse<BookingResponseDTO?>> CreateBookingOrderAsync(BookingOrderDTO order)
        {
            if (order == null)
            {
                return new ServiceResponse<BookingResponseDTO?>(null, InternalCode.InvalidParam);
            }

            /*
                Checks:
                - Check if all the flights are available
                - Check if the fares have available seats
                - If Checks are not passed, return 422
             */

            bool hasReturnFlight = !string.IsNullOrWhiteSpace(order.ReturnFlightNumber);
            int totalFlights = order.Bookings.Count;

            FlightInformation outboundFlightInfo = new FlightInformation();
            FlightInformation returnFlightInfo = new FlightInformation();

            var outboundFlight = CheckAvailableFlight(order.OutboundFlightNumber, totalFlights);

            //If flight not valid, return false
            if (outboundFlight.FlightInformation == null)
            {
                return new ServiceResponse<BookingResponseDTO?>(null, InternalCode.Unprocessable, "The selected flight is not valid");
            }

            if (outboundFlight.IsBookedToMax)
            {
                return new ServiceResponse<BookingResponseDTO?>(null, InternalCode.Unprocessable, "The number of flights exceed number of available seats");
            }

            outboundFlightInfo = outboundFlight.FlightInformation;

            //Check if Return flight available

            if (hasReturnFlight)
            {
                var returnFlight = CheckAvailableFlight(order.ReturnFlightNumber!, totalFlights);

                if (returnFlight.FlightInformation == null)
                {
                    return new ServiceResponse<BookingResponseDTO?>(null, InternalCode.Unprocessable, "The selected flight is not valid");
                }

                if (returnFlight.IsBookedToMax)
                {
                    return new ServiceResponse<BookingResponseDTO?>(null, InternalCode.Unprocessable, "The number of flights exceed number of available seats");
                }

                returnFlightInfo = returnFlight.FlightInformation;
            }

            //Check if the selected Fare has seats available
            var (IsAnyFareMaxedOut, FareCodes) = CheckIfAnyFareIsMaxedOut(order, hasReturnFlight);

            if (IsAnyFareMaxedOut)
            {
                return new ServiceResponse<BookingResponseDTO?>(null, InternalCode.Unprocessable, "The number of flights exceed number of available seats");
            }

            //if all checks passed, lets reduce number of available seats.
            //If Checks are passed, set status as InProgress and reduce available seats to avoid overbooking the flight
            //We hold a seat for N mins max(N is set in config). Use a background job to return UnbookedSeats back into the Pool

            await UpdateAvailableSeats(order, FareCodes);

            string orderReference = Guid.NewGuid().ToString("N")[..10].ToUpper();

            //Create all the bookings
            List<Booking> bookings = new List<Booking>();

            foreach (var booking in order.Bookings)
            {
                //TODO: Use AutoMapper
                bookings.Add(new Booking
                {
                    FirstName = booking.FirstName,
                    LastName = booking.LastName,
                    PhoneNumber = booking.PhoneNumber,
                    Email = booking.Email,
                    DateOfBirth = booking.DateOfBirth,
                    Address = booking.Address,
                    Gender = booking.Gender,
                    BookingNumber = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                    BookingStatus = BookingStatus.Pending,
                    FlightId = outboundFlightInfo.Id,
                    FlightFareId = booking.OutboundFlightFareId,
                    CreatedAt = DateTime.UtcNow
                });

                //If return flight, add a seperate booking
                if (hasReturnFlight)
                {
                    bookings.Add(new Booking
                    {
                        FirstName = booking.FirstName,
                        LastName = booking.LastName,
                        PhoneNumber = booking.PhoneNumber,
                        Email = booking.Email,
                        DateOfBirth = booking.DateOfBirth,
                        Address = booking.Address,
                        Gender = booking.Gender,
                        BookingNumber = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                        BookingStatus = BookingStatus.Pending,
                        FlightId = returnFlightInfo.Id,
                        FlightFareId = (int)booking.ReturnFlightFareId!,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            //Sum all cost as part of Order
            decimal orderCost = allFlightFares.Where(x => FareCodes.Contains(x.Id))
                .Sum(x => x.Price);

            BookingOrder bookingOrder = new BookingOrder
            {
                Bookings = bookings,
                Email = order.EmailAddress,
                OrderStatus = BookingStatus.Confirmed,
                TotalAmount = orderCost,
                OrderReference = orderReference,
                CreatedAt = DateTime.UtcNow,
                NumberOfAdults = 1,
                NumberOfChildren = 1,
            };

            int result = await _orderRepo.CreateAsync(bookingOrder);

            if (result != 1)
            {
                return new ServiceResponse<BookingResponseDTO?>(null, (InternalCode)result);
            }

            //Generate payment link with the cost if payment successful

            /*
             Return Order Number, Payment Link, Payment Expiry
             */
            StripeDataDTO stripeData = new StripeDataDTO
            {
                PaymentReference = Guid.NewGuid().ToString("N").ToUpper(),
                SuccessUrl = string.Empty,
                CancelUrl = string.Empty,
                ProductDescription = string.Empty,
                Amount = orderCost,
                CurrencyCode = "USD",
                CustomerEmail = order.EmailAddress,
                ProductName = "Flight Booking Service"
            };

            var stripeResponse = _stripeService.GetStripeCheckoutUrl(stripeData);

            BookingResponseDTO bookingResponse = new BookingResponseDTO
            {
                OrderReference = orderReference,
                PaymentLink = stripeResponse.Data
            };

            return new ServiceResponse<BookingResponseDTO?>(bookingResponse, InternalCode.Success);
        }

        public async Task<ServiceResponse<BookingResponseDTO?>> GetCheckoutUrlAsync(string orderReference)
        {
            if (string.IsNullOrWhiteSpace(orderReference))
            {
                return new ServiceResponse<BookingResponseDTO?>(null, InternalCode.InvalidParam, "Order reference not supplied");
            }

            //gett the order details
            var orderDetails = await _orderRepo.Query()
                .FirstOrDefaultAsync(x => x.OrderReference == orderReference);

            if (orderDetails == null)
            {
                return new ServiceResponse<BookingResponseDTO?>(null, InternalCode.EntityNotFound, "The order with the supplied order number was not found");
            }

            if (orderDetails.OrderStatus == BookingStatus.Paid)
            {
                return new ServiceResponse<BookingResponseDTO?>(null, InternalCode.Unprocessable, "This order has already been paid for");
            }

            StripeDataDTO stripeData = new StripeDataDTO
            {
                PaymentReference = Guid.NewGuid().ToString("N").ToUpper(),
                SuccessUrl = string.Empty,
                CancelUrl = string.Empty,
                ProductDescription = string.Empty,
                Amount = orderDetails.TotalAmount,
                CurrencyCode = "USD",
                CustomerEmail = orderDetails.Email,
                ProductName = "Flight Booking Service"
            };

            var stripeResponse = _stripeService.GetStripeCheckoutUrl(stripeData);

            BookingResponseDTO bookingResponse = new BookingResponseDTO
            {
                OrderReference = orderReference,
                PaymentLink = stripeResponse.Data
            };

            return new ServiceResponse<BookingResponseDTO?>(bookingResponse, InternalCode.Success);
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

        private (bool IsAnyFareMaxedOut, List<int> FareCodes) CheckIfAnyFareIsMaxedOut(BookingOrderDTO order, bool hasReturnFlight)
        {
            List<int> fareCodes = new List<int>();
            var outboundFares = order.Bookings.Select(x => x.OutboundFlightFareId).ToList();

            fareCodes.AddRange(outboundFares);
            if (hasReturnFlight)
            {
                //if the booking has no return flight fare, use the outbound flight fare
                foreach (var booking in order.Bookings)
                {
                    if (booking.ReturnFlightFareId == null)
                    {
                        fareCodes.Add(booking.OutboundFlightFareId);
                        continue;
                    }
                    fareCodes.Add(booking.OutboundFlightFareId!);
                }
            }

            //get all flight fares for all the list ID. We can then use throughout the booking process
            allFlightFares = _flightFareRepo.Query()
                .Where(x => fareCodes.Contains(x.Id))
                .ToList();

            var flightFares = allFlightFares
                .Select(y => new
                {
                    y.Id,
                    y.FareCode,
                    AvailableSeats = y.SeatCapacity - y.SeatReserved
                }).ToList();

            var fareGroup = fareCodes.GroupBy(x => x).ToList();
            bool isAnyFareMaxedOut = true;
            foreach (var group in fareGroup)
            {
                isAnyFareMaxedOut = flightFares.Any(x => x.Id == group.Key && group.Count() > x.AvailableSeats);
                if (isAnyFareMaxedOut)
                {
                    break;
                }
            }

            return (isAnyFareMaxedOut, fareCodes);
        }

        private async Task UpdateAvailableSeats(BookingOrderDTO order, List<int> fareCodes)
        {
            int totalFlights = order.Bookings.Count;

            int result = await _flightRepo.Query()
                .Where(x => x.FlightNumber == order.OutboundFlightNumber)
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.SeatReserved, y => y.SeatReserved + totalFlights));

            //If a return is booked, reduce the seats too
            if (!string.IsNullOrWhiteSpace(order.ReturnFlightNumber))
            {
                result = await _flightRepo.Query()
                    .Where(x => x.FlightNumber == order.ReturnFlightNumber)
                    .ExecuteUpdateAsync(x => x.SetProperty(y => y.SeatReserved, y => y.SeatReserved + totalFlights));
            }

            //update fare capacity. includes all fares, initial and return
            var grouped = fareCodes
                .GroupBy(x => x).ToList();

            foreach (var fare in grouped)
            {
                await _flightFareRepo.Query()
                    .Where(x => x.Id == fare.Key)
                    .ExecuteUpdateAsync(x => x.SetProperty(y => y.SeatReserved, y => y.SeatReserved + fare.Count()));
            }

            return;
        }
    }
}
