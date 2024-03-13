using FlightBooking.Service.Data.DTO;
using Stripe;

namespace FlightBooking.Service.Services.Interfaces
{
    public interface IStripeService
    {
        ServiceResponse<string> GetStripeCheckoutUrl(StripeDataDTO stripeDataDTO);
        Task<ServiceResponse<string>> ProcessPayment(Event stripeEvent);
    }
}