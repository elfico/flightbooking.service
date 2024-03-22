using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Service.Data.DTO
{
    public class StripeDataDTO
    {
        [Required]
        public string SuccessUrl { get; set; } = null!;

        [Required]
        public string CancelUrl { get; set; } = null!;

        [Required]
        public string ProductName { get; set; } = null!;

        [Required]
        public string ProductDescription { get; set; } = null!;

        [Required]
        [Range(0.5, double.MaxValue)] //minimum of 50 cents
        public decimal Amount { get; set; }

        [Required]
        public string CustomerEmail { get; set; } = null!;

        [Required]
        public string CurrencyCode { get; set; } = "USD";

        [Required]
        public string OrderNumber { get; set; } = null!;
    }
}