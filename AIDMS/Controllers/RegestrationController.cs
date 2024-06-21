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
using AIDMS.Security_Entities;

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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private static string _nationalId = null;
        // Temporary storage for documents
        private readonly Dictionary<int, List<AIDocument>> _tempDocumentStorage = new();

        public RegestrationController(IStudentRepository studentRepository,AIDMSContextClass context,IUniversityListNIdsRepository universityListNIds, IDocumentRepository documentRepository,
            IConfiguration configuration, IGoogleCloudVisionRepository visionRepository, IGoogleCloudStorageRepository storageRepository,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _studentRepository = studentRepository;
            _context = context;
            _universityListNIds = universityListNIds;
            _documentRepository = documentRepository;
            _configuration = configuration;
            _visionRepository = visionRepository;
            _storageRepository = storageRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("validate-national-id")]
        public async Task<IActionResult> ValidateNationalID(string NationalId)
        {
            var existingNationalId = await _universityListNIds.CheckExistanceOfNationalId(NationalId);
            if (existingNationalId == null)
                return BadRequest("National ID not found");

            var existingStudent = await _studentRepository.GetStudentByNationalIdAsync(NationalId);

            if (existingStudent != null && existingStudent.regestrationStatus == true)
            {
                return Ok("You're Accepted!");
            }
            if (existingStudent != null && existingStudent.regestrationStatus == false)
            {
                return Ok("Your request is pending");
            }
            _nationalId = NationalId;
            return Ok("Your id is Valid!");
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now < dateOfBirth.AddYears(age))
                age--;
            return age;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] StudentRegisterationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_nationalId == null)
                return BadRequest("You must add National Id in step 1 first");

            // Check if the email already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest("Email already in use");

            var existingUsername = await _userManager.FindByNameAsync(model.Username);
            if (existingUsername != null)
                return BadRequest("username already in use");

            // Handling userManagerProbs
            ApplicationUser applicationUser = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                NationalId = _nationalId
            };

            var result = await _userManager.CreateAsync(applicationUser, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Create Cookie
            await _userManager.AddToRoleAsync(applicationUser, "Student");
            await _signInManager.SignInAsync(applicationUser, isPersistent: false);

            // Step 4: Create Student Record without storing the plain password
            var student = new Student
            {
                firstName = model.firstName, // From Google Vision Model
                lastName = model.lastName, // From Google Vision Model
                TotalPassedHours = 0,
                Level = 1,
                PhoneNumber = model.PhoneNumber,
                militaryEducation = false,
                regestrationStatus = null,
                DepartmentId = 1,
                userName = model.Username,
                dateOfBirth = model.dateOfBirth,
                GPA = 0,
                IsMale = model.isMale,
                studentPicture = model.profilePicture,
                Age = CalculateAge(model.dateOfBirth)
            };
            student.SID = _nationalId;

            bool? ok = await _studentRepository.AddStudentAsync(student);
            if (ok == false)
                return BadRequest("Error, Please Check The Info Again!");

            return Ok(new { Message = "Successful Registration" });
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
            student.regestrationStatus = true;
            return Ok("Application submitted successfully.");
        }
    }
}