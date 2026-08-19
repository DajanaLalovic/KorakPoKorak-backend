using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KorakPoKorak.API.Controllers
{
    [ApiController]
    [Route("api/media")]
    [Authorize(Roles = "Administrator,Mentor")]
    public class MediaController : ControllerBase
    {
        private const long MaxUploadBytes = 20 * 1024 * 1024; // 20 MB

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp", ".gif",
            ".mp3", ".m4a", ".ogg", ".wav", ".aac",
            ".mp4", ".webm",
            ".pdf", ".doc", ".docx", ".odt", ".txt", ".csv"
        };

        private readonly IFileStorageService _fileStorage;

        public MediaController(IFileStorageService fileStorage)
        {
            _fileStorage = fileStorage;
        }

        /// <summary>
        /// Uploads a content file (image/audio/document) for Lesson/Exercise blocks.
        /// Multipart form field name: file. Returns a server URL to put in contentBlocks[].url.
        /// </summary>
        [HttpPost("upload")]
        [RequestSizeLimit(MaxUploadBytes)]
        public IActionResult Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File is required." });

            if (file.Length > MaxUploadBytes)
                return BadRequest(new { message = "File must be 20 MB or smaller." });

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
                return BadRequest(new { message = "File type is not allowed." });

            try
            {
                using var stream = file.OpenReadStream();
                var url = _fileStorage.SaveContentFile(stream, extension);
                return Ok(new MediaUploadResultDto
                {
                    Url = url,
                    MimeType = string.IsNullOrWhiteSpace(file.ContentType)
                        ? null
                        : file.ContentType,
                    FileName = file.FileName
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
