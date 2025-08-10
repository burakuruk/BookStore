using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace BookStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public FilesController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file.");
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg", ".jpeg", ".png", ".webp"
            };
            var ext = Path.GetExtension(file.FileName);
            if (!allowed.Contains(ext)) return BadRequest("Invalid file type.");
            // bazı tarayıcılar/istemciler content-type göndermeyebilir, o durumda uzantıyı esas al
            if (!string.IsNullOrEmpty(file.ContentType) && !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid mime type.");
            var imagesDir = Path.Combine(_env.ContentRootPath, "wwwroot", "images");
            Directory.CreateDirectory(imagesDir);
            var fileName = Path.GetRandomFileName() + ext.ToLowerInvariant();
            var path = Path.Combine(imagesDir, fileName);
            await using var stream = System.IO.File.Create(path);
            await file.CopyToAsync(stream);
            var publicUrl = $"/images/{fileName}";
            return Ok(new { url = publicUrl });
        }
    }
}


