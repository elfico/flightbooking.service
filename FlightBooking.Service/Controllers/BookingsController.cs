using FlightBooking.Service.Data.DTO;
using FlightBooking.Service.Services;
using FlightBooking.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FlightBooking.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IBookingOrderService _bookingOrderService;
        public BookingsController(IBookingService bookingService, IBookingOrderService bookingOrderService)
        {
            _bookingService = bookingService;
            _bookingOrderService = bookingOrderService;
        }

        [HttpGet("bookingNumber/{bookingNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetBookingsByBookingNumber([FromRoute] string bookingNumber)
        {
            ServiceResponse<BookingDTO?> result = await _bookingService.GetBookingByBookingNumberAsync(bookingNumber);

            return result.FormatResponse();
        }

        [HttpGet("email/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookingDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public IActionResult GetBookingsByEmail([FromRoute] string email)
        {
            ServiceResponse<IEnumerable<BookingDTO>?> result = _bookingService.GetBookingsByEmail(email);

            return result.FormatResponse();
        }


        [HttpGet("{bookingId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetBookingById([FromRoute] int bookingId)
        {
            ServiceResponse<BookingDTO?> result = await _bookingService.GetBookingByBookingId(bookingId);

            return result.FormatResponse();
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingResponseDTO))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> PostBooking([FromBody] BookingOrderDTO bookingOrderDTO)
        {
            ServiceResponse<BookingResponseDTO?> result = await _bookingOrderService.CreateBookingOrderAsync(bookingOrderDTO);

            return result.FormatResponse();
        }

        [HttpGet("payment")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingResponseDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetBookingPayment([FromRoute] string orderReference)
        {
            ServiceResponse<BookingResponseDTO?> result = await _bookingOrderService.GetCheckoutUrlAsync(orderReference);

            return result.FormatResponse();
        }
    }
}
