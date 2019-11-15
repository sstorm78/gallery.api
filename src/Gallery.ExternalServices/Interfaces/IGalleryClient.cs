using System.Collections.Generic;
using System.Threading.Tasks;
using Gallery.ExternalServices.Typicode.Models;

namespace Gallery.ExternalServices.Interfaces
{
    public interface IGalleryClient
    {
        Task<List<Photo>> GetPhotosByUserId(int userId);
        Task<List<Album>> GetAlbumsByUserId(int userId);
    }
}
