using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Service.Data.Models
{
    public class Flights
    {
        [Key]
        public int Id { get; set; }

        public string FlightNumber { get; set; } = null!;
        public string Origin { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string Airline { get; set; } = null!;
        public int SeatCapacity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<FlightFares> FlightFares { get; set; } = new List<FlightFares>();
        public ICollection<ReservedSeat> ReservedSeats { get; set; } = new List<ReservedSeat>();
    }
}