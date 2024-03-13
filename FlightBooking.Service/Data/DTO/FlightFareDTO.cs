namespace FlightBooking.Service.Data.DTO
{
    public class FlightFareDTO
    {
        public int Id { get; set; }
        public string FareCode { get; set; } = null!;
        public string FareName { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal AvailableSeats { get; set; }

        public string FlightNumber { get; set; } = null!;
    }
}
