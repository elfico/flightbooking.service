namespace FlightBooking.Service.Data.Configs
{
    public static class ConfigSettingsModule
    {
        public static void AddConfigSettings(this IServiceCollection services, IConfiguration configuration)
        {
            //services.Configure<EmailConfig>(configuration.GetSection(EmailConfig.ConfigName));
        }
    }
}