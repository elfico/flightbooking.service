using FlightBooking.Service.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightBooking.Service.Data.ModelConfigurations
{
    public class BookingOrderConfiguration : IEntityTypeConfiguration<BookingOrder>
    {
        public void Configure(EntityTypeBuilder<BookingOrder> entity)
        {
            //entity.Property(e => e.Version).IsRowVersion();

            entity.Property(e => e.VersionGuid).IsConcurrencyToken();
        }
    }
}