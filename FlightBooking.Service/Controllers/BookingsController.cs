using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        public BookingsController() { }

        [HttpGet("{bookingNumber}")]
        public IActionResult GetBookingsByBookingNumber([FromRoute] string bookingNumber)
        {
            return Ok();
        }

        [HttpGet("email/{email}")]
        public IActionResult GetBookingsByEmail([FromRoute] string email)
        {
            return Ok();
        }


        [HttpPost]
        public IActionResult PostBooking()
        {
            return Ok();
        }
    }
}
