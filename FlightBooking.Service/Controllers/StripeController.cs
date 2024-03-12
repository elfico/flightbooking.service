using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        public StripeController() { }

        [HttpGet]
        public IActionResult GeneratePaymentLink()
        {
            return Ok();
        }

        //webhook for stripe to verify payment
        [HttpPost]
        public async Task<IActionResult> NotificationWebhookAsync()
        {
            return Ok();
        }
    }
}
