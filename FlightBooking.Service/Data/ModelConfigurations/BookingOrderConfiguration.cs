using FlightBooking.Service.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlightBooking.Service.Data.ModelConfigurations
{
    public class BookingOrderConfiguration : IEntityTypeConfiguration<BookingOrder>
    {
        public void Configure(EntityTypeBuilder<BookingOrder> entity)
        {
        }
    }
}