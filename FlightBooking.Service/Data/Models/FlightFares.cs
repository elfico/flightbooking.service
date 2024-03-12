using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Service.Data.Models
{
    public class FlightFares
    {
        [Key]
        public int Id { get; set; }
        public string FareCode { get; set; } = null!;
        public string FareName { get; set; } = null!;
        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        //FK to Flights
    }
}