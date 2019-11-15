using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gallery.Models;
using Gallery.Services.Factories;

namespace Gallery.Services
{
    public class GalleryService : IGalleryService
    {
        private readonly ITypicodeClientFactory _typicodeClientFactory;

        public GalleryService(ITypicodeClientFactory typicodeClientFactory)
        {
            _typicodeClientFactory = typicodeClientFactory ?? throw new ArgumentNullException(nameof(typicodeClientFactory));
        }

        public async Task<IList<Album>> GetAlbumsByUser(int userId)
        {
            var result = new List<Album>();

            var apiClient = _typicodeClientFactory.GetClient();

            var albums = apiClient.GetAlbumsByUserId(userId);
            var photos = apiClient.GetPhotosByUserId(userId);

            await Task.WhenAll(albums, photos);

            if (albums.Result == null || !albums.Result.Any())
            {
                return result;
            }

            Parallel.ForEach(albums.Result, (album) =>
                                    {
                                        result.Add(
                                            new Album(album.Id,
                                                album.Title,
                                                photos.Result.Where(i => i.AlbumId == album.Id)
                                                    .Select(p => new Photo(p.Id, p.Title, p.Url, p.ThumbnailUrl)).ToList()));
                                    });

            return result;
        }
    }
}
