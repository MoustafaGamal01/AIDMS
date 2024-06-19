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
        private readonly IPDFManagementRepository _pDFManagementRepository;

        //private readonly IGoogleCloudVisionRepository _visionService;

        public VisionController(/*IGoogleCloudVisionRepository visionService*/ IPDFManagementRepository PDFManagementRepository)
        {
            _pDFManagementRepository = PDFManagementRepository;
            //_visionService = visionService;
        }

        #region Google Vision
        //[HttpPost("analyze")]
        //public async Task<IActionResult> AnalyzeDocument([FromForm] IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("Invalid file.");

        //    using var stream = new MemoryStream();
        //    await file.CopyToAsync(stream);
        //    var image = Image.FromBytes(stream.ToArray());

        //    var response = await _visionService.AnalyzeDocumentAsync(image);
        //    return Ok(response);
        //}

        //[HttpPost("NationalId", Name = "validateNationalId")]
        //public async Task<IActionResult> ValidateNationalId([FromForm] string imagePath)
        //{
        //    //var featureList = new List<Feature>
        //    //{
        //    //    new Feature { Type = Feature.Types.Type.TextDetection },
        //    //    new Feature { Type = Feature.Types.Type.FaceDetection },
        //    //    new Feature { Type = Feature.Types.Type.LabelDetection }
        //    //};

        //    try
        //    {
        //        var nationalIdValidationScore = await _visionService.CheckNationalIdValidationAsync(imagePath);
        //        return Ok(new { ValidationScore = nationalIdValidationScore });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Error = ex.Message });
        //    }
        //}

        ////[HttpPost("BirthCertificate", Name = "validateBirthCertificate")]
        ////public async Task<IActionResult> ValidateBirthCertificate([FromForm] string imagePath)
        ////{
        ////    var featureList = new List<Feature>
        ////    {
        ////        new Feature { Type = Feature.Types.Type.TextDetection },
        ////        new Feature { Type = Feature.Types.Type.FaceDetection },
        ////        new Feature { Type = Feature.Types.Type.LabelDetection }
        ////    };

        ////    try
        ////    {
        ////        var nationalIdValidationScore = await _visionService.CheckBirthDateCertificateValidationAsync(featureList, imagePath);
        ////        return Ok(new { ValidationScore = nationalIdValidationScore });
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        return StatusCode(500, new { Error = ex.Message });
        ////    }
        ////}

        ////[HttpPost("SecondaryCertificate", Name = "validateSecondaryCertificate")]
        ////public async Task<IActionResult> ValidateSecondaryCertificate([FromForm] string imagePath)
        ////{
        ////    var featureList = new List<Feature>
        ////    {
        ////        new Feature { Type = Feature.Types.Type.TextDetection },
        ////        new Feature { Type = Feature.Types.Type.FaceDetection },
        ////        new Feature { Type = Feature.Types.Type.LabelDetection }
        ////    };

        ////    try
        ////    {
        ////        var nationalIdValidationScore = await _visionService.CheckSecondaryCertificateValidationAsync(imagePath);
        ////        return Ok(new { ValidationScore = nationalIdValidationScore });
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        return StatusCode(500, new { Error = ex.Message });
        ////    }
        ////} 
        #endregion

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeDocument(IFormFile file)
        {
            string str = await _pDFManagementRepository.ReadPDFContent(file);
        
            return Ok($"str: {str}");
        }
    }
}
