using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Service.Data.Models
{
    public class ReservedSeat
    {
        [Key]
        public int Id { get; set; }

        public string SeatNumber { get; set; } = null!; // e.g 1A, 33B
        public string? BookingNumber { get; set; }
        public int? BookingId { get; set; }  //FK to Booking
        public string FlightNumber { get; set; } = null!;
        public int FlightInformationId { get; set; }
        public bool IsReserved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual FlightInformation FlightInformation { get; set; } = null!;
        public virtual Booking? Booking { get; set; }
    }
}