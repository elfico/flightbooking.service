using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Service.Data.Models
{
    public class BookingOrder
    {
        [Key]
        public int Id { get; set; }

        public string OrderNumber { get; set; } = null!;
        public string Email { get; set; } = null!;

        [Precision(19, 4)]
        public decimal TotalAmount { get; set; }

        public BookingStatus OrderStatus { get; set; } = BookingStatus.Pending;
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}