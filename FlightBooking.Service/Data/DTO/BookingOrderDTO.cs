using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Service.Data.DTO
{
    public class BookingOrderDTO
    {
        [Required]
        public string EmailAddress { get; set; } = null!;

        [Required]
        public string FlightNumber { get; set; } = null!;

        public string? ReturnFlightNumber { get; set; }

        public List<BookingRequestDTO> Bookings { get; set; } = new List<BookingRequestDTO>();
    }
}
