using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Service.Data.Models
{
    public class BookingOrder
    {
        [Key]
        public int Id { get; set; }
        public string OrderReference { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public BookingStatus OrderStatus { get; set; } = BookingStatus.Pending;
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
