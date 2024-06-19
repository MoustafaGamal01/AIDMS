using AIDMS.Entities;
using AIDMS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Vision.V1;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AIDMS.DTOs;

namespace AIDMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegestrationController : ControllerBase
    {
        private readonly AIDMSContextClass _context;
        private readonly IStudentRepository _studentRepository;
        private readonly IUniversityListNIdsRepository _universityListNIds;
        private readonly IDocumentRepository _documentRepository;
        private readonly IConfiguration _configuration;
        private readonly IGoogleCloudVisionRepository _visionRepository;
        private readonly IGoogleCloudStorageRepository _storageRepository;

        // Temporary storage for documents
        private readonly Dictionary<int, List<AIDocument>> _tempDocumentStorage = new();

        public RegestrationController(IStudentRepository studentRepository,AIDMSContextClass context,IUniversityListNIdsRepository universityListNIds, IDocumentRepository documentRepository,
            IConfiguration configuration, IGoogleCloudVisionRepository visionRepository, IGoogleCloudStorageRepository storageRepository)
        {
            _studentRepository = studentRepository;
            _context = context;
            _universityListNIds = universityListNIds;
            _documentRepository = documentRepository;
            _configuration = configuration;
            _visionRepository = visionRepository;
            _storageRepository = storageRepository;
        }

        [HttpPost("validate-national-id/{NationalId:alpha}")]
        public async Task<IActionResult> ValidateNationalID(string NationalId)
        {
            var existingStudent = await _universityListNIds.CheckExistanceOfNationalId(NationalId);
            if (existingStudent == null)
                return BadRequest("National ID not found");
            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Step 1: Check National ID
            var existingStudent = _context.UniversityListNIds.FirstOrDefault(x => x.NationalId == model.NationalID);
            if (existingStudent == null)
                return BadRequest("National ID not found");

            // Step 2: Create User
            var student = new Student
            {
                userName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                SID = model.NationalID
            };

            bool? ok = await _studentRepository.AddStudentAsync(student);
            //_userManager.CreateAsync(student, model.Password);
            if (ok == false)
                return BadRequest("Error, Please Check The Info Again!");

            return Ok(new { Message = "Registration successful" });
        }

        [HttpPost("upload-document")]
        public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentDto uploadDocumentDto)
        {
            {
                if (uploadDocumentDto.file == null || uploadDocumentDto.file.Length == 0)
                    return BadRequest("Invalid file");

                var student = await _studentRepository.GetAllStudentDataByIdAsync(uploadDocumentDto.studentId); 
                if (student == null)
                    return NotFound("Student not found");


                // Step 4: Upload to (TempBucket) Google Cloud Storage
                var fileUrl = await _storageRepository.UploadFileAsync(uploadDocumentDto.file);

                var validationScore = 0.0;

                if (uploadDocumentDto.step == 3)
                {
                    validationScore = await _visionRepository.CheckNationalIdValidationAsync(fileUrl);
                }
                else if (uploadDocumentDto.step == 4)
                {
                    validationScore = await _visionRepository.CheckBirthDateCertificateValidationAsync(fileUrl);
                }
                else if (uploadDocumentDto.step == 5)
                {
                    validationScore = await _visionRepository.CheckSecondaryCertificateValidationAsync(fileUrl);
                }

                if (validationScore < 70) // assuming 70% is the threshold
                    return BadRequest($"Sorry, Document validation failed!");
                
                // Step 4: Store Document Information in Dictionary
                var document = new AIDocument
                {
                    FileName = uploadDocumentDto.file.FileName,
                    FileType = uploadDocumentDto.file.ContentType,
                    FilePath = fileUrl,
                    UploadedAt = DateTime.UtcNow,
                    StudentId = student.Id
                };

                if (!_tempDocumentStorage.ContainsKey(uploadDocumentDto.studentId))
                {
                    _tempDocumentStorage[uploadDocumentDto.studentId] = new List<AIDocument>();
                }
                _tempDocumentStorage[uploadDocumentDto.studentId].Add(document);

                return Ok(new { Message = "Document uploaded and validated successfully", DocumentUrl = fileUrl });
            }
        }

        [HttpPost("submit-application/{studentId}")]
        public async Task<IActionResult> SubmitApplication(int studentId)
        {
             var student =  await _studentRepository.GetAllStudentDataByIdAsync(studentId);
            if (student == null)
                return NotFound("Student not found");

            // Check if all required documents are uploaded
            if (!_tempDocumentStorage.ContainsKey(studentId) || _tempDocumentStorage[studentId].Count != 6)
            {
                return BadRequest("Incomplete registration process. Please complete all steps.");
            }

            var documents = _tempDocumentStorage[studentId];

            // Create a new application
            var application = new Application
            {
                Title = "Student Application",
                Status = "Pending",
                isAccepted = false,
                SubmittedAt = DateTime.UtcNow,
                DecisionDate = DateTime.UtcNow.AddMonths(1), // Example decision date
                ReviewDate = DateTime.UtcNow.AddDays(7), // Example review date
                StudentId = studentId,
                Documents = documents
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            // Update document references with the application ID
            foreach (var document in documents)
            {
                document.ApplicationId = application.Id;
                await _documentRepository.AddDocumentAsync(document);
            }

            // Remove documents from temporary storage
            _tempDocumentStorage.Remove(studentId);

            return Ok("Application submitted successfully.");
        }
    }
}