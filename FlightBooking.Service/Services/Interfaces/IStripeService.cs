using FlightBooking.Service.Data.DTO;
using Stripe;

namespace FlightBooking.Service.Services.Interfaces
{
    public interface IStripeService
    {
        /// <summary>
        /// Creates a Stripe payment link
        /// </summary>
        /// <param name="stripeDataDTO"></param>
        /// <returns>Payment link</returns>
        ServiceResponse<string> GetStripeCheckoutUrl(StripeDataDTO stripeDataDTO);

        /// <summary>
        /// Processes Stripe events when a checkout (payment) is completed
        /// </summary>
        /// <param name="stripeEvent"></param>
        /// <returns></returns>
        Task<ServiceResponse<string>> ProcessPayment(Event stripeEvent);
    }
}