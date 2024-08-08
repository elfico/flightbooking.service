using FlightBooking.Service.Data;
using NLog;
using NLog.Web;
using OpenTelemetry.Resources;

namespace FlightBooking.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

            try
            {
                logger.Debug("init main");

                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    try
                    {
                        DatabaseSeeding.Initialize(services);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "An error occurred seeding the DB.");
                    }
                }

                host.Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    //logging.AddConsole();
                    logging.AddOpenTelemetry(opt =>
                    {
                        opt.SetResourceBuilder(ResourceBuilder.CreateDefault()
                            .AddService("Flight Service"));
                    });
                    //logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
                });
            //.UseNLog();  // NLog: Setup NLog for Dependency injection
        }
    }
}