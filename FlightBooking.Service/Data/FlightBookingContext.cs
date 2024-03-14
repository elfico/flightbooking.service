using FlightBooking.Service.Data.ModelConfigurations;
using FlightBooking.Service.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Service.Data
{
    public class FlightBookingContext : DbContext
    {
        public FlightBookingContext(DbContextOptions<FlightBookingContext> options) : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingOrder> BookingOrders { get; set; }
        public DbSet<ReservedSeat> ReservedSeats { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<FlightFare> FlightFares { get; set; }
        public DbSet<FlightInformation> FlightInformation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //configure each model

            modelBuilder.ApplyConfiguration(new FlightInformationConfiguration());
            modelBuilder.ApplyConfiguration(new FlightFareConfiguration());
            modelBuilder.ApplyConfiguration(new BookingConfiguration());
            modelBuilder.ApplyConfiguration(new BookingOrderConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new ReservedSeatConfiguration());

            ////Ensure all dates are saved as UTC and read as UTC:
            ////https://github.com/dotnet/efcore/issues/4711#issuecomment-481215673

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        modelBuilder.Entity(entityType.ClrType)
                         .Property<DateTime>(property.Name)
                         .HasConversion(
                          v => v.ToUniversalTime(),
                          v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        modelBuilder.Entity(entityType.ClrType)
                         .Property<DateTime?>(property.Name)
                         .HasConversion(
                          v => v.HasValue ? v.Value.ToUniversalTime() : v,
                          v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
                    }
                }
            }
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}