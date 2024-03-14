namespace FlightBooking.Service.Data.Configs
{
    public class ConfigSettings
    {
    }

    public class StripeConfig
    {
        public const string ConfigName = nameof(StripeConfig);

        public string SecretKey { get; set; } = null!;
        public string PublicKey { get; set; } = null!;
        public string SigningSecret { get; set; } = null!;
    }
}