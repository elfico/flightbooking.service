using FlightBooking.Service.Data.Configs;
using FlightBooking.Service.Services;
using FlightBooking.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace FlightBooking.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private readonly StripeConfig _stripeConfig;
        private readonly IStripeService _stripeService;

        public StripeController(IOptionsMonitor<StripeConfig> options, IStripeService stripeService)
        {
            _stripeConfig = options.CurrentValue;
            _stripeService = stripeService;
        }

        //webhook for stripe to verify payment
        [HttpPost]
        public async Task<IActionResult> NotificationWebhookAsync()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            string stripeSecretKey = _stripeConfig.SigningSecret;

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], stripeSecretKey, throwOnApiVersionMismatch: false);
            }
            catch (StripeException)
            {
                return BadRequest();
            }

            //Since this is the only event we are handling.
            if (stripeEvent.Type == Events.CheckoutSessionCompleted)
            {
                var response = await _stripeService.ProcessPayment(stripeEvent);

                return response.FormatResponse();
            }

            return Ok();
        }
    }
}
