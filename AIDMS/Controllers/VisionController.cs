using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Vision.V1;
using System.Threading.Tasks;
using System.IO;
using AIDMS.Repositories;

namespace AIDMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisionController : ControllerBase
    {
        private readonly IGoogleCloudVisionService _visionService;

        public VisionController(IGoogleCloudVisionService visionService)
        {
            _visionService = visionService;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeDocument([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            var image = Image.FromBytes(stream.ToArray());

            var response = await _visionService.AnalyzeDocumentAsync(image);
            return Ok(response);
        }
    }
}
