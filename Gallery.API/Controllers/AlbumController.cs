using System;
using System.Threading.Tasks;
using Gallery.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.API.Controllers
{
    [Route("albums")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly IGalleryService _galleryService;

        public AlbumController(IGalleryService galleryService)
        {
            _galleryService = galleryService ?? throw new ArgumentNullException(nameof(galleryService));
        }

        // GET /albums?userId=xxx
        [HttpGet]
        public async Task<IActionResult> GetAlbumsByUser(int userId)
        {
            try
            {
                var result = await _galleryService.GetAlbumsByUser(userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                //no logging on purpose
                //no other condition handling to simplify the test task
                return NotFound();
            }
        }
    }
}