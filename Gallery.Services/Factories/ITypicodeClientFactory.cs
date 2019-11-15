using Gallery.ExternalServices.Interfaces;

namespace Gallery.Services.Factories
{
    public interface ITypicodeClientFactory
    {
        IGalleryClient GetClient();
    }
}