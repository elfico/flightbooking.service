using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatsController : ControllerBase
    {
        public IActionResult GetAvailableSeats()
        {
            return Ok();
        }

        public IActionResult ReserveSeat()
        {
            return Ok();
        }
    }
}
