using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Service.Data.DTO
{
    public class BookingOrderDTO
    {
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string OutboundFlightNumber { get; set; } = null!;

        public string? ReturnFlightNumber { get; set; }

        [Required]
        public List<BookingRequestDTO> Bookings { get; set; } = new List<BookingRequestDTO>();
    }

    public class BookingResponseDTO
    {
        public string OrderReference { get; set; } = null!;
        public string PaymentLink { get; set; } = null!;
        public DateTime OrderExpiration { get; set; } = DateTime.UtcNow.AddHours(1);
    }
}
