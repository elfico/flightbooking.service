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

        public string BookNumber { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual FlightInformation InitialFlight { get; set; } = null!;
        public virtual FlightInformation? ReturnFlight { get; set; }

        public virtual FlightFare InitialFlightFare { get; set; } = null!;
        public virtual FlightFare? ReturnFlightFare { get; set; }

        public virtual BookingOrder BookingOrder { get; set; } = null!;

    }
}