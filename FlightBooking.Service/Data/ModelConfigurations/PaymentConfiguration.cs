using FlightBooking.Service.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightBooking.Service.Data.ModelConfigurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> entity)
        {
            entity.HasOne(d => d.BookingOrder).WithMany(p => p.Payments)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.BookingOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}