using System.Collections.Generic;
using System.Threading.Tasks;
using Gallery.ExternalServices.Interfaces;
using Gallery.Models;
using Album = Gallery.ExternalServices.Typicode.Models.Album;
using Photo = Gallery.ExternalServices.Typicode.Models.Photo;

namespace Gallery.ExternalServices.Typicode
{
    public class TypiCodeClient:ClientBase, IGalleryClient
    {
        public TypiCodeClient(IConfig config)
        :base(config.ExternalServicesTypiCodeUrl, config.ExternalServicesTypiCodeTimeoutSeconds)
        {
        }

        public async Task<List<Photo>> GetPhotosByUserId(int userId)
        {
            var url = "photos?userId=" + userId;
            return await Get<List<Photo>>(url);
        }

        public async Task<List<Album>> GetAlbumsByUserId(int userId)
        {
            var url = "albums?userId=" + userId;
            return await Get<List<Album>>(url);
        }
    }
}
