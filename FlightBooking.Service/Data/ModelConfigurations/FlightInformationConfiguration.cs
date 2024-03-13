using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Service.Data.Models;

namespace FlightBooking.Service.Data.ModelConfigurations
{
    public class FlightInformationConfiguration : IEntityTypeConfiguration<FlightInformation>
    {
        public void Configure(EntityTypeBuilder<FlightInformation> entity)
        {
            
        }
    }
}
