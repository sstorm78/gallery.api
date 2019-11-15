using Gallery.ExternalServices.Interfaces;
using Gallery.ExternalServices.Typicode;
using Gallery.Models;

namespace Gallery.Services.Factories
{
    public class TypicodeClientFactory : ITypicodeClientFactory
    {
        private readonly IConfig _config;

        public TypicodeClientFactory(IConfig config)
        {
            _config = config;
        }

        public IGalleryClient GetClient()
        {
            return new TypiCodeClient(_config);
        }
    }
}
