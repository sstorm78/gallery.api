using System.Collections.Generic;
using System.Threading.Tasks;
using Gallery.Models;

namespace Gallery.Services
{
    public interface IGalleryService
    {
        Task<IList<Album>> GetAlbumsByUser(int userId);
    }
}