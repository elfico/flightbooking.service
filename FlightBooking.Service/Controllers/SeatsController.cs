using FlightBooking.Service.Data.DTO;
using FlightBooking.Service.Services;
using FlightBooking.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FlightBooking.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly IReservedSeatService _service;

        public SeatsController(IReservedSeatService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReservedSeatDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public IActionResult GetAvailableSeats([FromQuery] string flightNumber)
        {
            ServiceResponse<IEnumerable<ReservedSeatDTO>> result = _service.GetAvailableSeatsByFlightNumber(flightNumber);

            return result.FormatResponse();
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ReserveSeat([FromBody] ReservedSeatRequestDTO requestDTO)
        {
            ServiceResponse<string> result = await _service.ReserveSeatAsync(requestDTO);

            return result.FormatResponse();
        }
    }
}