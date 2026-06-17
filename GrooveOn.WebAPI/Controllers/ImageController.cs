using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GrooveOn.Services.Interfaces;
using GrooveOn.Services.Database;

namespace GrooveOn.WebAPI.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly GrooveOnDbContext _context;

        public ImageController(IImageService imageService, GrooveOnDbContext context)
        {
            _imageService = imageService;
            _context = context;
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

        [Authorize(Roles = Roles.UserAndAdmin)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string folder, [FromQuery] string fileName, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return BadRequest("fileName is required.");

            var isAdmin = User.IsInRole(Roles.Admin);

            if (!isAdmin)
            {
                var userId = GetCurrentUserId();
                var normalizedFolder = (folder ?? "").Trim().ToLowerInvariant();

                var allowed = normalizedFolder switch
                {
                    "users" => await _context.Users
                        .AnyAsync(u => u.Id == userId && u.UserImage == fileName, ct),
                    "playlists" => await _context.Playlists
                        .AnyAsync(p => p.UserId == userId && p.CoverImageUrl == fileName, ct),
                    _ => false
                };

                if (!allowed)
                    return Forbid();
            }

            var ok = await _imageService.DeleteAsync(fileName, folder, ct);
            return ok ? Ok(new { deleted = true }) : NotFound(new { deleted = false });
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var userId))
                throw new UnauthorizedAccessException("Invalid token.");
            return userId;
        }
    }
}
