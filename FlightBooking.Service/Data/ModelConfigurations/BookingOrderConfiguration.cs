using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Service.Data.Models;

namespace FlightBooking.Service.Data.ModelConfigurations
{
    public class BookingOrderConfiguration : IEntityTypeConfiguration<BookingOrder>
    {
        public void Configure(EntityTypeBuilder<BookingOrder> entity)
        {
            

        }
    }
}
