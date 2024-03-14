namespace FlightBooking.Service.Data.DTO
{
    public class ReservedSeatDTO
    {
        public string SeatNumber { get; set; } = null!; // e.g 1A, 33B

        //public string BookingNumber { get; set; } = null!;
        public string FlightNumber { get; set; } = null!;

        public bool IsReserved { get; set; }
    }

    public class ReservedSeatRequestDTO
    {
        public string SeatNumber { get; set; } = null!; // e.g 1A, 33B
        public string BookingNumber { get; set; } = null!;
    }
}