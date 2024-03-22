using FlightBooking.Service.Data;
using FlightBooking.Service.Data.DTO;
using FlightBooking.Service.Data.Models;
using FlightBooking.Service.Data.Repository;
using FlightBooking.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Service.Services
{
    ///<inheritdoc />
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
                - outbound and return flights cannot be same
                - Check if all the flights are available
                - Check if the fares have available seats
                - If Checks are not passed, return 422
             */

            bool hasReturnFlight = !string.IsNullOrWhiteSpace(order.ReturnFlightNumber);

            //Outbound and Return flights cannot be the same
            if (hasReturnFlight && order.ReturnFlightNumber == order.OutboundFlightNumber)
            {
                return new ServiceResponse<BookingResponseDTO?>(null, InternalCode.Unprocessable, "Outbound and return flights cannot be the same");
            }

            int totalFlights = order.Bookings.Count;

            FlightInformation outboundFlightInfo = new FlightInformation();
            FlightInformation returnFlightInfo = new FlightInformation();

            //Check if the outbound flight is valid and if seats are available
            var outboundFlight = CheckAvailableFlight(order.OutboundFlightNumber, totalFlights);

            //If flight not valid, return false
            if (outboundFlight.FlightInformation == null)
            {
                return new ServiceResponse<BookingResponseDTO?>(null, InternalCode.Unprocessable, "The selected flight is not valid");
            }

            //if it's booked to Max, return error
            if (outboundFlight.IsBookedToMax)
            {
                return new ServiceResponse<BookingResponseDTO?>(null, InternalCode.Unprocessable, "The number of flights exceed number of available seats");
            }

            outboundFlightInfo = outboundFlight.FlightInformation;

            //Check if Return flight available and not booked to max

            if (hasReturnFlight)
            {
                //If none of the Booking

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

            //Check if the selected Fares for each flight is valid and seats are available
            var (IsAllFareExists, IsAnyFareMaxedOut, FareCodes) = CheckIfAnyFareIsMaxedOut(order, hasReturnFlight);

            if (!IsAllFareExists)
            {
                return new ServiceResponse<BookingResponseDTO?>(null, InternalCode.Unprocessable, "Some selected fare do not exist. Please check that all fare exists");
            }

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

            decimal totalAmount = 0;
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
                    BookingStatus = BookingStatus.Confirmed,
                    FlightId = outboundFlightInfo.Id,
                    FlightFareId = booking.OutboundFareId,
                    CreatedAt = DateTime.UtcNow
                });

                totalAmount += allFlightFares.FirstOrDefault(x => x.Id == booking.OutboundFareId)!.Price;

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
                        BookingStatus = BookingStatus.Confirmed,
                        FlightId = returnFlightInfo.Id,
                        FlightFareId = (int)booking.ReturnFareId!,
                        CreatedAt = DateTime.UtcNow
                    });

                    totalAmount += allFlightFares.FirstOrDefault(x => x.Id == booking.ReturnFareId)!.Price;
                }
            }

            //Sum all cost as part of Order

            BookingOrder bookingOrder = new BookingOrder
            {
                Bookings = bookings,
                Email = order.Email,
                OrderStatus = BookingStatus.Confirmed,
                TotalAmount = totalAmount,
                OrderNumber = orderReference,
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
                SuccessUrl = "https://localhost:44321/success",
                CancelUrl = "https://localhost:44321/cancel",
                ProductDescription = $"Booking for {orderReference}",
                Amount = totalAmount,
                CurrencyCode = "USD",
                CustomerEmail = order.Email,
                ProductName = "Flight Booking Service",
                OrderNumber = orderReference,
            };

            var stripeResponse = _stripeService.GetStripeCheckoutUrl(stripeData);

            BookingResponseDTO bookingResponse = new BookingResponseDTO
            {
                OrderNumber = orderReference,
                PaymentLink = stripeResponse.Data
            };

            return new ServiceResponse<BookingResponseDTO?>(bookingResponse, InternalCode.Success);
        }

        public async Task<ServiceResponse<BookingResponseDTO?>> GetCheckoutUrlAsync(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                return new ServiceResponse<BookingResponseDTO?>(null, InternalCode.InvalidParam, "Order reference not supplied");
            }

            //gett the order details
            var orderDetails = await _orderRepo.Query()
                .FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);

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
                SuccessUrl = "https://localhost:44321/success",
                CancelUrl = "https://localhost:44321/cancel",
                ProductDescription = $"Booking for Order : {orderDetails.OrderNumber}",
                Amount = orderDetails.TotalAmount,
                CurrencyCode = "USD",
                CustomerEmail = orderDetails.Email,
                ProductName = "Flight Booking Service",
                OrderNumber = orderNumber
            };

            var stripeResponse = _stripeService.GetStripeCheckoutUrl(stripeData);

            BookingResponseDTO bookingResponse = new BookingResponseDTO
            {
                OrderNumber = orderNumber,
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

        private (bool IsAllFareExists, bool IsAnyFareMaxedOut, List<int> FareCodes) CheckIfAnyFareIsMaxedOut(BookingOrderDTO order, bool hasReturnFlight)
        {
            //get all flight fares for all the list ID. We can then use throughout the booking process
            var validFlightFares = _flightFareRepo.Query()
                .Include(x => x.FlightInformation)
                .Where(x => x.FlightInformation.FlightNumber == order.OutboundFlightNumber);

            //if return flight, then add the fares
            if (hasReturnFlight)
            {
                validFlightFares = _flightFareRepo.Query()
                .Include(x => x.FlightInformation)
                .Where(x => x.FlightInformation.FlightNumber == order.OutboundFlightNumber
                    || x.FlightInformation.FlightNumber == order.ReturnFlightNumber);
            }

            //we get all the flight fares and save to the variable so we can reuse
            allFlightFares = validFlightFares.ToList();

            List<int> fareIds = new List<int>();

            //get all fare Id
            var outboundFares = order.Bookings
                .Select(x => x.OutboundFareId)
                .ToList();

            //Add to a list
            fareIds.AddRange(outboundFares);

            //we check if all fares are valid for that flight
            var isAllFareValid = allFlightFares.Any(x => outboundFares.Contains(x.Id)
                    && x.FlightInformation.FlightNumber == order.OutboundFlightNumber);

            //if atleast one of the fare in the outbound flight is invalid, return false
            if (!isAllFareValid)
            {
                return (false, true, new List<int>());
            }

            //check the codes for the return flights are also valid
            if (hasReturnFlight)
            {
                var returnFares = order.Bookings
                    .Select(x => (int)x.ReturnFareId!)
                    .ToList();

                fareIds.AddRange(returnFares);

                isAllFareValid = allFlightFares.Any(x => returnFares.Contains(x.Id)
                         && x.FlightInformation.FlightNumber == order.ReturnFlightNumber);

                if (!isAllFareValid)
                {
                    return (false, true, new List<int>());
                }
            }

            var flightFares = allFlightFares
                .Select(y => new
                {
                    y.Id,
                    y.FareCode,
                    AvailableSeats = y.SeatCapacity - y.SeatReserved
                }).ToList();

            //group the fares by Id
            var fareGroup = fareIds.GroupBy(x => x).ToList();

            bool isAnyFareMaxedOut = true;

            //for each fare, check if the seats are available
            foreach (var group in fareGroup)
            {
                isAnyFareMaxedOut = flightFares.Any(x => x.Id == group.Key && group.Count() > x.AvailableSeats);

                //if any fare is maxed out, terminate the loop
                if (isAnyFareMaxedOut)
                {
                    break;
                }
            }

            return (true, isAnyFareMaxedOut, fareIds);
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
                result = await _flightFareRepo.Query()
                    .Where(x => x.Id == fare.Key)
                    .ExecuteUpdateAsync(x => x.SetProperty(y => y.SeatReserved, y => y.SeatReserved + fare.Count()));
            }

            return;
        }
    }
}