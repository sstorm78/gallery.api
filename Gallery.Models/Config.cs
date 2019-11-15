using System;
using Microsoft.Extensions.Configuration;

namespace Gallery.Models
{
    public interface IConfig
    {
        string ExternalServicesTypiCodeUrl { get; }
        int ExternalServicesTypiCodeTimeoutSeconds { get; }
    }

    public class Config: IConfig
    {
        private readonly IConfigurationRoot _configurationRoot;

        public Config(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot ?? throw new ArgumentNullException(nameof(configurationRoot));
        }

        public string ExternalServicesTypiCodeUrl => (_configurationRoot["ExternalServices:TypiCode:Url"]);
        public int ExternalServicesTypiCodeTimeoutSeconds => (int.Parse(_configurationRoot["ExternalServices:TypiCode:timeoutSeconds"]));
    }
}
