using FlightBooking.Service.Data;
using FlightBooking.Service.Data.Configs;
using FlightBooking.Service.Data.DTO;
using FlightBooking.Service.Data.Models;
using FlightBooking.Service.Data.Repository;
using FlightBooking.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;

namespace FlightBooking.Service.Services
{
    ///<inheritdoc />
    public class StripeService : IStripeService
    {
        private readonly StripeConfig _stripeConfig;
        private readonly IGenericRepository<Payment> _paymentRepo;
        private readonly IGenericRepository<BookingOrder> _orderRepo;

        private readonly ILogger<StripeService> _logger;

        public StripeService(IOptionsMonitor<StripeConfig> options, IGenericRepository<Payment> paymentRepo,
            IGenericRepository<BookingOrder> orderRepo, ILogger<StripeService> logger)
        {
            _stripeConfig = options.CurrentValue;
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
            _logger = logger;
        }

        public ServiceResponse<string> GetStripeCheckoutUrl(StripeDataDTO stripeDataDTO)
        {
            if (stripeDataDTO == null)
            {
                return new ServiceResponse<string>(string.Empty, InternalCode.InvalidParam, "Invalid Data");
            }

            string checkoutUrl = string.Empty;

            try
            {
                StripeConfiguration.ApiKey = _stripeConfig.SecretKey;

                var amountInCents = stripeDataDTO.Amount * 100;

                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = stripeDataDTO.CurrencyCode,
                        UnitAmountDecimal = amountInCents, //in cents
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = stripeDataDTO.ProductName,
                            Description = stripeDataDTO.ProductDescription
                        }
                    },
                    Quantity = 1,
                  },
                },
                    Mode = "payment",
                    SuccessUrl = stripeDataDTO.SuccessUrl,
                    CancelUrl = stripeDataDTO.CancelUrl,
                    ClientReferenceId = stripeDataDTO.PaymentReference,
                    CustomerEmail = stripeDataDTO.CustomerEmail,
                };
                var service = new SessionService();
                Session session = service.Create(options);
                checkoutUrl = session.Url;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.ToString());
            }

            return new ServiceResponse<string>(checkoutUrl, InternalCode.Success);
        }

        public async Task<ServiceResponse<string>> ProcessPayment(Event stripeEvent)
        {
            string orderNumber = string.Empty;
            string bookingNumber = string.Empty;

            var session = stripeEvent.Data.Object as Session;

            //check if payment already saved, if yes, return
            bool isPaymentSaved = _paymentRepo.Query()
                .Any(x => x.PaymentReference == session!.ClientReferenceId);

            if (isPaymentSaved)
            {
                return new ServiceResponse<string>(string.Empty, InternalCode.Success);
            }

            //save payment
            Payment payment = new Payment
            {
                TransactionDate = session!.Created,
                OrderNumber = session.Id,
                MetaData = JsonConvert.SerializeObject(session),
                BookingOrderId = Convert.ToInt32(session.Id),
                CurrencyCode = session.Currency,
                CustomerEmail = session.CustomerEmail,
                PaymentReference = session.ClientReferenceId,
                PaymentStatus = session.PaymentStatus,
                CreatedAt = DateTime.UtcNow,
                TransactionAmount = (decimal)session.AmountTotal!,
            };

            await _paymentRepo.CreateAsync(payment);

            //update flight and booking information
            var bookingOrder = await _orderRepo.Query()
                .Include(x => x.Bookings)
                .FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);

            if (bookingOrder == null)
            {
                _logger.LogCritical("Payment made for a booking that doesn't exist");
                return new ServiceResponse<string>(string.Empty, InternalCode.Success);
            }

            bookingOrder.OrderStatus = BookingStatus.Paid;

            foreach (var booking in bookingOrder.Bookings)
            {
                booking.BookingStatus = BookingStatus.Paid;
            }

            await _orderRepo.SaveChangesToDbAsync();

            return new ServiceResponse<string>(string.Empty, InternalCode.Success);
        }
    }
}