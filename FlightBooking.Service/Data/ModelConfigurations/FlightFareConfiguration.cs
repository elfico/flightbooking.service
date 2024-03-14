using FlightBooking.Service.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightBooking.Service.Data.ModelConfigurations
{
    public class FlightFareConfiguration : IEntityTypeConfiguration<FlightFare>
    {
        public void Configure(EntityTypeBuilder<FlightFare> entity)
        {
            entity.HasOne(d => d.FlightInformation).WithMany(p => p.FlightFares)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.FlightInformationId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}