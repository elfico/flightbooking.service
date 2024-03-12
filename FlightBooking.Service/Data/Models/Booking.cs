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
        public string FlightNumber { get; set; } = null!;
        public string BookingNumber { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Origin { get; set; } = null!;
        public string Destination { get; set; } = null!;

        public int NumberOfAdults;

        public int NumberOfChildren;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<FlightFares> FlightFares { get; set; } = new List<FlightFares>();
    }
}