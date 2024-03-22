using FlightBooking.Service.Data.Configs;
using FlightBooking.Service.Services;
using FlightBooking.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using System.Text;

namespace FlightBooking.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private readonly StripeConfig _stripeConfig;
        private readonly IStripeService _stripeService;
        private readonly ILogger<StripeController> _logger;
        public StripeController(IOptionsMonitor<StripeConfig> options, IStripeService stripeService, ILogger<StripeController> logger)
        {
            _stripeConfig = options.CurrentValue;
            _stripeService = stripeService;
            _logger = logger;
        }

        //webhook for stripe to verify payment
        [HttpPost]
        public async Task<IActionResult> NotificationWebhookAsync()
        {
            //var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            string requestBody;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrEmpty(requestBody))
            {
                //_logger.LogInformation("Event object is empty", eventObject);
                return BadRequest();
            }

            string stripeSigningKey = _stripeConfig.SigningSecret;

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(requestBody, Request.Headers["Stripe-Signature"], stripeSigningKey, throwOnApiVersionMismatch: false);
            }
            catch (StripeException exception)
            {
                _logger.LogError(exception.ToString());
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