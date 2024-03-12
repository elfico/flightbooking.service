﻿using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Service.Data.Models
{
    public class Payments
    {
        [Key]
        public int Id { get; set; }
        public string CustomerEmail { get; set; } = null!;
        public decimal TransactionAmount { get; set; }
        public string PaymentReference { get; set; } = null!;
        public string BookingNumber { get; set; } = null!;
        public string CurrencyCode { get; set; } = null!;
        public string PaymentChannel { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public DateTime TransactionDate { get; set; }
        public string? MetaData {  get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual BookingOrder BookingOrder { get; set; } = null!;
    }
}