using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Service.Data.Models;

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
