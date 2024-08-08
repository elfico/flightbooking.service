using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetTestInformation()
        {
            string testInfo = "We the best";

            _logger.LogInformation(testInfo);

            return Ok(testInfo);
        }
    }
}
