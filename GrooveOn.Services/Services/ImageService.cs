using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using GrooveOn.Services.Interfaces;
using GrooveOn.Services.Exceptions;

namespace GrooveOn.Services.Services
{
    public class ImageService : IImageService
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        private static readonly Dictionary<string, byte[]> MagicBytes = new()
        {
            { ".jpg",  new byte[] { 0xFF, 0xD8, 0xFF } },
            { ".jpeg", new byte[] { 0xFF, 0xD8, 0xFF } },
            { ".png",  new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },
            { ".webp", new byte[] { 0x52, 0x49, 0x46, 0x46 } }
        };

        private static readonly Dictionary<string, string[]> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            { ".jpg", ["image/jpeg", "image/pjpeg"] },
            { ".jpeg", ["image/jpeg", "image/pjpeg"] },
            { ".png", ["image/png"] },
            { ".webp", ["image/webp"] }
        };

        private const long MaxBytes = 10 * 1024 * 1024;

        private readonly IWebHostEnvironment _env;

        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveAsync(
            IFormFile file,
            string nameOfTheFolder,
            string? desiredFileName = null,
            CancellationToken ct = default)
        {
            if (file == null || file.Length == 0)
                throw new UserException("No file was provided.");

            if (file.Length > MaxBytes)
                throw new UserException("Image is too large. Maximum allowed size is 10 MB.");

            var folder = NormalizeFolder(nameOfTheFolder);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
                throw new UserException("Invalid image format. Allowed formats: jpg, jpeg, png, webp.");

            ValidateMimeType(file, ext);
            await ValidateMagicBytesAsync(file, ext, ct);

            var safeFileName = MakeSafeFileName(desiredFileName);
            if (string.IsNullOrWhiteSpace(safeFileName))
            {
                safeFileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}{ext}";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Path.GetExtension(safeFileName)))
                    safeFileName += ext;

                var desiredExt = Path.GetExtension(safeFileName);
                if (!AllowedExtensions.Contains(desiredExt))
                    safeFileName = Path.ChangeExtension(safeFileName, ext);
            }

            var physicalFolder = GetPhysicalFolder(folder);
            Directory.CreateDirectory(physicalFolder);

            var fullPath = Path.Combine(physicalFolder, safeFileName);

            await using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(stream, ct);

            return safeFileName;
        }

        public Task<bool> DeleteAsync(string fileName, string nameOfTheFolder, CancellationToken ct = default)
        {
            var folder = NormalizeFolder(nameOfTheFolder);

            var safeName = MakeSafeFileName(fileName);
            if (string.IsNullOrWhiteSpace(safeName))
                return Task.FromResult(false);

            var physicalFolder = GetPhysicalFolder(folder);
            var fullPath = Path.Combine(physicalFolder, safeName);

            if (!File.Exists(fullPath))
                return Task.FromResult(false);

            File.Delete(fullPath);
            return Task.FromResult(true);
        }

        public string GetPublicUrl(string fileName, string nameOfTheFolder)
        {
            var folder = NormalizeFolder(nameOfTheFolder);
            var safeName = MakeSafeFileName(fileName);
            return $"/images/{folder}/{safeName}";
        }

        private string GetPhysicalFolder(string folder)
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");
            return Path.Combine(webRoot, "images", folder);
        }

        private static string NormalizeFolder(string folder)
        {
            return (folder ?? "").Trim().ToLowerInvariant();
        }

        private static string MakeSafeFileName(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            var name = input.Trim();

            name = name.Replace("\\", "/");
            name = Path.GetFileName(name);

            name = Regex.Replace(name, @"[^a-zA-Z0-9._-]", "");

            if (name is "." or "..") return "";

            return name;
        }

        private static async Task ValidateMagicBytesAsync(IFormFile file, string ext, CancellationToken ct)
        {
            if (!MagicBytes.TryGetValue(ext, out var expected))
                throw new UserException("Invalid image format.");

            var buffer = new byte[expected.Length];
            using var stream = file.OpenReadStream();
            var read = await stream.ReadAsync(buffer, 0, buffer.Length, ct);

            if (read < expected.Length)
                throw new UserException("File is too small to be a valid image.");

            for (int i = 0; i < expected.Length; i++)
            {
                if (buffer[i] != expected[i])
                    throw new UserException("File content does not match the declared image format.");
            }

            if (ext == ".webp")
            {
                // WebP: bytes 8-11 must be "WEBP"
                var webpBuffer = new byte[12];
                stream.Seek(0, SeekOrigin.Begin);
                var webpRead = await stream.ReadAsync(webpBuffer, 0, 12, ct);
                if (webpRead < 12 ||
                    webpBuffer[8] != 0x57 || webpBuffer[9] != 0x45 ||
                    webpBuffer[10] != 0x42 || webpBuffer[11] != 0x50)
                    throw new UserException("File content does not match the declared image format.");
            }
        }

        private static void ValidateMimeType(IFormFile file, string ext)
        {
            if (!AllowedMimeTypes.TryGetValue(ext, out var allowedMimeTypes))
                throw new UserException("Invalid image format.");

            var contentType = file.ContentType?.Split(';')[0].Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(contentType) ||
                !allowedMimeTypes.Contains(contentType))
            {
                throw new UserException("File content type does not match the declared image format.");
            }
        }
    }
}
