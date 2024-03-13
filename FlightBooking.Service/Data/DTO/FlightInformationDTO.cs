namespace FlightBooking.Service.Data.DTO
{
    public class FlightInformationDTO: BookingFlightInformationDTO
    {
        public int Id { get; set; }
        public int SeatCapacity { get; set; }
        public int AvailableSeats { get; set; }
    }

    public class BookingFlightInformationDTO
    {
        public string FlightNumber { get; set; } = null!;
        public string Origin { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string Airline { get; set; } = null!;
    }
}
