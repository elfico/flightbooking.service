using FlightBooking.Service.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightBooking.Service.Data.ModelConfigurations
{
    public class FlightInformationConfiguration : IEntityTypeConfiguration<FlightInformation>
    {
        public void Configure(EntityTypeBuilder<FlightInformation> entity)
        {
        }
    }
}