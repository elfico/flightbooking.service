using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Service.Data.Models
{
    public class ReservedSeat
    {
        [Key]
        public int Id { get; set; }
        public string SeatId { get; set; } = null!; // e.g 1A, 33B
        public string BookingNumber { get; set; } = null!;
        public string FlightNumber { get; set; } = null!;
        public bool IsReserved { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}