using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Service.Data.Models
{
    public class FlightFare
    {
        [Key]
        public int Id { get; set; }
        public string FareCode { get; set; } = null!;
        public string FareName { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal Capacity { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public virtual FlightInformation FlightInformation { get; set; } = null!;

        //FK to Flights
    }
}