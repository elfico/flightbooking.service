using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Service.Data.Models;

namespace FlightBooking.Service.Data.ModelConfigurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> entity)
        {
            entity.HasOne(d => d.BookingOrder).WithMany(p => p.Bookings)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.BookingOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FlightInformation).WithMany(p => p.Bookings)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FlightFare).WithMany(p => p.Bookings)
               .HasPrincipalKey(p => p.Id)
               .HasForeignKey(d => d.FlightFareId)
               .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
