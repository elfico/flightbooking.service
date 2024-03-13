using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Service.Data.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }

        public string BookingNumber { get; set; } = null!;
        public int BookingOrderId { get; set; }
        public BookingStatus BookingStatus { get; set; } = BookingStatus.Pending;

        public int FlightId { get; set; }
        public int FlightFareId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual FlightInformation FlightInformation  { get; set; } = null!;
        public virtual FlightFare FlightFare { get; set; } = null!;
        public virtual BookingOrder BookingOrder { get; set; } = null!;

    }
}