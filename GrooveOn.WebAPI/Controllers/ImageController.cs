using Microsoft.AspNetCore.Mvc;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GrooveOn.WebAPI.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [Authorize(Roles = Roles.UserAndAdmin)]
        [HttpPost("upload")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Upload([FromQuery] string folder, IFormFile file, [FromQuery] string? fileName = null, CancellationToken ct = default)
        {
            if (file == null) return BadRequest("File is required.");

            var savedName = await _imageService.SaveAsync(file, folder, fileName, ct);
            var url = _imageService.GetPublicUrl(savedName, folder);

            return Ok(new
            {
                fileName = savedName,
                url                
            });
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string folder, [FromQuery] string fileName, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return BadRequest("fileName is required.");

            var ok = await _imageService.DeleteAsync(fileName, folder, ct);
            return ok ? Ok(new { deleted = true }) : NotFound(new { deleted = false });
        }
    }
}
