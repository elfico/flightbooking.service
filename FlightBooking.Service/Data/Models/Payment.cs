using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Service.Data.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public string CustomerEmail { get; set; } = null!;

        [Precision(19, 4)]
        public decimal TransactionAmount { get; set; }
        public string PaymentReference { get; set; } = null!;
        public string OrderNumber { get; set; } = null!;
        public string CurrencyCode { get; set; } = null!;
        public string PaymentChannel { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public int BookingOrderId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? MetaData { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual BookingOrder BookingOrder { get; set; } = null!;
    }
}