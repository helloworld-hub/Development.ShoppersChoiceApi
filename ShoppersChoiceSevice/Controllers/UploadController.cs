using Azure.Core;
using Microsoft.AspNetCore.Mvc;

namespace ShoppersChoiceSevice.Controllers
{
    [ApiController]
    [Route("shopperschoiceservice/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly string _uploadPath;

        public UploadController(IConfiguration configuration)
        {
            _uploadPath = configuration["UploadSettings:UploadPath"]
                ?? throw new Exception("Upload path not configured");
        }

        [HttpPost("images")]
        public async Task<IActionResult> UploadImages(List<IFormFile> images)
        {
            if (images == null || images.Count == 0)
                return BadRequest("No images selected");

            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);

            var imageUrls = new List<string>();

            foreach (var image in images)
            {
                if (image.Length == 0) continue;

                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(_uploadPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream);

                var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                imageUrls.Add(imageUrl);
            }

            return Ok(imageUrls);
        }
    }
}
