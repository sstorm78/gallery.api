namespace Gallery.ExternalServices.Tests
{
    public class TestConfig : Models.IConfig
    {
        public string ExternalServicesTypiCodeUrl { get; set; }

        public int ExternalServicesTypiCodeTimeoutSeconds { get; set; }
    }
}
