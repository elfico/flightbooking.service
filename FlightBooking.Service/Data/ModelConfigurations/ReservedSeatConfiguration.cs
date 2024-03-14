using FlightBooking.Service.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightBooking.Service.Data.ModelConfigurations
{
    public class ReservedSeatConfiguration : IEntityTypeConfiguration<ReservedSeat>
    {
        public void Configure(EntityTypeBuilder<ReservedSeat> entity)
        {
            entity.HasOne(d => d.FlightInformation).WithMany(p => p.ReservedSeats)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.FlightInformationId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}