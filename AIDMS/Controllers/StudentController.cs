using AIDMS.DTOs;
using AIDMS.Entities;
using AIDMS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using MimeKit;
using static System.Net.Mime.MediaTypeNames;
using Google.Apis.Auth.OAuth2.Requests;
using Microsoft.AspNetCore.Authorization;

namespace AIDMS.Controllers
{
    [Authorize(Roles = "Student")]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly AIDMSContextClass _context;
        private readonly IStudentRepository _student;
        private readonly IApplicationRepository _application;
        private readonly INotificationRepository _notification;
        private readonly IGoogleCloudStorageRepository _googleCloud;
        private readonly IPaymentRepository _payment;

        public StudentController(AIDMSContextClass context, IStudentRepository student, IApplicationRepository application,
            INotificationRepository notification, IWebHostEnvironment hostingEnvironment, IDocumentRepository document,
            IGoogleCloudStorageRepository googleCloud, IPaymentRepository payment)
        {
            this._context = context;
            this._student = student;
            this._application = application;
            this._notification = notification;
            this._googleCloud = googleCloud;
            this._payment = payment;
        }
        [HttpGet]
        [Route("{Id:int}", Name = "StudentPersonalInfo")]
        public async Task<IActionResult> GetStudentPersonalInfoById(int Id)
        {
            Student std = await _student.GetStudentPersonalInfoByIdAsync(Id);
            if (std == null)
            {
                return NotFound("There's no Student with this Id");
            }
            if (ModelState.IsValid)
            {
                StudentDto studentDto = new StudentDto();
                studentDto.Id = std.Id;
                studentDto.GPA = std.GPA;
                studentDto.TotalPassedHours = std.TotalPassedHours;
                studentDto.Level = std.Level;
                studentDto.firstName = std.firstName;
                studentDto.lastName = std.lastName;
                studentDto.studentDepartment = std.Department.Name;
                studentDto.studentDocuments = std.Documents;
                studentDto.studentPicture = std.studentPicture;
                return Ok(studentDto);
            }
            return BadRequest();
        }
        [Authorize(Roles = "Student")]

        [HttpGet]
        [Route("notifications/{studentId:int}")]
        public async Task<IActionResult> GetStudentNotifications(int studentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var notifications = await _notification.GetAllNotificationsByStudentIdAsync(studentId);

            if (notifications == null)
            {
                return NotFound();
            }

            var notificationsListDto = notifications.Select(n => new UserNotificationDto
            {
                userPicture = n.Employee.employeePicture,
                userFirstName = n.Employee.firstName,
                userLastName = n.Employee.lastName,
                Message = n.Message,
                CreatedAt = n.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            }).ToList();

            return Ok(notificationsListDto);
        }

        [HttpGet]
        [Route("settings/{studentId:int}")]
        public async Task<IActionResult> GetStudentSettings(int studentId)
        {
            var student = await _student.GetAllStudentDataByIdAsync(studentId);
            if (student == null)
            {
                return NotFound("Student not found");
            }
            if (ModelState.IsValid)
            {
                UserSettingsDto userSettingsDto = new UserSettingsDto();
                userSettingsDto.userName = student.userName; 
                userSettingsDto.Phone = student.PhoneNumber;
                userSettingsDto.profilePicture = student.studentPicture;
                return Ok(userSettingsDto);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("pending/{studentId:int}")]
        public async Task<IActionResult> GetPendingApplicationsByStudentIdAsync(int studentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var applications = await _application.GetAllPendingApplicationsByStudentIdAsync(studentId);

            if (applications == null)
            {
                return NotFound();
            }

            var applicationsDto = applications.Select(n => new StudentApplicationDto
            {
                Id = n.Id,
                documentName = n.Title,
                status = n.Status,
                uploadedAt = n.SubmittedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            }).ToList();

            return Ok(applicationsDto);
        }

        [HttpGet]
        [Route("applications/{studentId:int}")]
        public async Task<IActionResult> GetAllApplicationsByStudentIdAsync(int studentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var applications = await _application.GetAllApplicationsByStudentIdAsync(studentId);

            if (applications == null)
            {
                return NotFound();
            }

            var applicationsDto = applications.Select(n => new StudentApplicationDto
            {
                Id = n.Id,
                documentName = n.Title,
                status = n.Status,
                uploadedAt = n.SubmittedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            }).ToList();

            return Ok(applicationsDto);
        }

        [HttpGet]
        [Route("archived/{studentId:int}")]
        public async Task<IActionResult> GetArchivedApplicationsByStudentIdAsync(int studentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var applications = await _application.GetAllArchivedApplicationsByStudentIdAsync(studentId);

            if (applications == null)
            {
                return NotFound();
            }

            var applicationsDto = applications.Select(n => new StudentApplicationDto
            {
                Id = n.Id,
                documentName = n.Title,
                status = n.Status,
                uploadedAt = n.SubmittedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                isAccepted = n.isAccepted    
                
            }).ToList();

            return Ok(applicationsDto);
        }

        [HttpPost]
        [Route("material/{employeeId:int}")]
        public async Task<IActionResult> RequestMatrialRegestration([FromForm] CreateApplicationDto applicationDto, int employeeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (applicationDto.StudentDocument?.Length > 1048576) // Max size 1 MB in bytes
            {
                return BadRequest("File size cannot exceed 1 MB.");
            }

            // Get the file type and extension
            var fileType = GetFileType(applicationDto.StudentDocument);
            var fileExtension = GetFileExtension(fileType);

            // Generate a unique filename for student's document with extension
            var student = await _student.GetStudentPersonalInfoByIdAsync(applicationDto.StudentId);
            var studentFileName = GenerateUniqueFileName(student.firstName + '_' + student.lastName) + fileExtension;

            // Save student's uploaded document to Google Cloud Storage (if any) and handle success/failure
            string fileUrl = null;
            if (applicationDto.StudentDocument != null)
            {
                try
                {
                    fileUrl = await _googleCloud.UploadFileAsync(applicationDto.StudentDocument);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload document: {ex.Message}");
                }
            }

            // Create the application entity with details
            var application = new AIDMS.Entities.Application
            {
                Title = "Matrial Regestration",
                Description = applicationDto.Description,
                Status = "Pending",
                StudentId = applicationDto.StudentId,
                SubmittedAt = DateTime.Now,
                isAccepted = false,
                EmployeeId = employeeId
            };

            if (applicationDto.StudentDocument != null)
            {
                application.Documents = new List<AIDocument>
                {
                    new AIDocument
                    {
                        FileName = studentFileName, // Use the generated unique filename with extension
                        FileType = fileType, // Get the actual file type
                        FilePath = fileUrl, // Use the URL from Google Cloud Storage
                        UploadedAt = DateTime.Now,
                        StudentId = applicationDto.StudentId,
                    }
                };
            }

            // Save the application entity first
            await _application.AddApplicationAsync(application);

            // Create and save a mock Payment entity linked to the application
            var payment = new Payment
            {
                DocumentURL = fileUrl,
                Amount = 100.00m, // Mock amount
                TimeStamp = DateTime.Now,
                //ApplicationId = application.Id // Link the payment to the application
            };

            await _payment.AddPaymentAsync(payment);

            // Update the application with the PaymentId
            application.PaymentId = payment.Id;
            var applicationEntity = await _application.GetApplicationByIdAsync(application.Id);
            await _application.UpdateApplicationAsync(applicationEntity.Id, application);


            // Get the name of the student
            var std = await _student.GetStudentPersonalInfoByIdAsync(applicationDto.StudentId);

            // Create notifications for employees about new application
            await _notification.AddNotificationAsync(new Notification
            {
                Message = $"Student: {std.firstName} {std.lastName} - ID: {applicationDto.StudentId} \n Requested a new application: Matrial Regestration",
                EmployeeId = employeeId,
                AIDocumentId = application.Documents?.FirstOrDefault()?.Id,
                CreatedAt = DateTime.Now,
                fromStudent = true,
                StudentId = applicationDto.StudentId,
            });

            return CreatedAtRoute("StudentPersonalInfo", new { Id = applicationDto.StudentId }, application);
        }

        [HttpPost]
        [Route("expenses")]
        public async Task<IActionResult> RequestExpensesPayment([FromForm] CreateApplicationDto applicationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (applicationDto.StudentDocument?.Length > 1048576) // Max size 1 MB in bytes
            {
                return BadRequest("File size cannot exceed 1 MB.");
            }

            // Get the file type and extension
            var fileType = GetFileType(applicationDto.StudentDocument);
            var fileExtension = GetFileExtension(fileType);

            // Generate a unique filename for student's document with extension
            var student = await _student.GetStudentPersonalInfoByIdAsync(applicationDto.StudentId);
            var studentFileName = GenerateUniqueFileName(student.firstName + '_' + student.lastName) + fileExtension;

            // Save student's uploaded document to Google Cloud Storage (if any) and handle success/failure
            string fileUrl = null;
            if (applicationDto.StudentDocument != null)
            {
                try
                {
                    fileUrl = await _googleCloud.UploadFileAsync(applicationDto.StudentDocument);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload document: {ex.Message}");
                }
            }

            // Create the application entity with details
            var application = new AIDMS.Entities.Application
            {
                Title = "Expenses Payment",
                Description = applicationDto.Description,
                Status = "Pending",
                StudentId = applicationDto.StudentId,
                SubmittedAt = DateTime.Now,
                isAccepted = false
            };

            if (applicationDto.StudentDocument != null)
            {
                application.Documents = new List<AIDocument>
                {
                    new AIDocument
                    {
                        FileName = studentFileName, // Use the generated unique filename with extension
                        FileType = fileType, // Get the actual file type
                        FilePath = fileUrl, // Use the URL from Google Cloud Storage
                        UploadedAt = DateTime.Now,
                        StudentId = applicationDto.StudentId,
                    }
                };
            }

            // Save the application entity first
            await _application.AddApplicationAsync(application);

            // Create and save a mock Payment entity linked to the application
            var payment = new Payment
            {
                DocumentURL = fileUrl,
                Amount = 100.00m, // Mock amount
                TimeStamp = DateTime.Now,
                //ApplicationId = application.Id // Link the payment to the application
            };

            await _payment.AddPaymentAsync(payment);
            
            // Update the application with the PaymentId
            application.PaymentId = payment.Id;
            var applicationEntity = await _application.GetApplicationByIdAsync(application.Id);
            await _application.UpdateApplicationAsync(applicationEntity.Id, application);


            // Get the name of the student
            var std = await _student.GetStudentPersonalInfoByIdAsync(applicationDto.StudentId);

            // Create notifications for employees about new application
            await _notification.AddNotificationAsync(new Notification
            {
                Message = $"Student: {std.firstName} {std.lastName} - ID: {applicationDto.StudentId} \n submitted a new application: Expenses Payment",
                //EmployeeId = 6,
                AIDocumentId = application.Documents?.FirstOrDefault()?.Id,
                CreatedAt = DateTime.Now,
                fromStudent = true,
                StudentId = applicationDto.StudentId,
            });

            return CreatedAtRoute("StudentPersonalInfo", new { Id = applicationDto.StudentId }, application);
        }

        [HttpPost]
        [Route("military")]
        public async Task<IActionResult> RequestMilitaryEducation([FromForm] CreateApplicationDto applicationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (applicationDto.StudentDocument?.Length > 1048576) // Max size 1 MB in bytes
            {
                return BadRequest("File size cannot exceed 1 MB.");
            }

            // Get the file type and extension
            var fileType = GetFileType(applicationDto.StudentDocument);
            var fileExtension = GetFileExtension(fileType);

            // Generate a unique filename for student's document with extension
            var student = await _student.GetStudentPersonalInfoByIdAsync(applicationDto.StudentId);
            var studentFileName = GenerateUniqueFileName(student.firstName + '_' + student.lastName) + fileExtension;

            // Save student's uploaded document to Google Cloud Storage (if any) and handle success/failure
            string fileUrl = null;
            if (applicationDto.StudentDocument != null)
            {
                try
                {
                    fileUrl = await _googleCloud.UploadFileAsync(applicationDto.StudentDocument);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload document: {ex.Message}");
                }
            }

            // Create the application entity with details
            var application = new AIDMS.Entities.Application
            {
                Title = "Military Education",
                Description = applicationDto.Description,
                Status = "Pending",
                StudentId = applicationDto.StudentId,
                SubmittedAt = DateTime.Now,
                isAccepted = false
            };

            if (applicationDto.StudentDocument != null)
            {
                application.Documents = new List<AIDocument>
                {
                    new AIDocument
                    {
                        FileName = studentFileName, // Use the generated unique filename with extension
                        FileType = fileType, // Get the actual file type
                        FilePath = fileUrl, // Use the URL from Google Cloud Storage
                        UploadedAt = DateTime.Now,
                        StudentId = applicationDto.StudentId,
                    }
                };
            }

            // Save the application entity first
            await _application.AddApplicationAsync(application);

            // Create and save a mock Payment entity linked to the application
            var payment = new Payment
            {
                DocumentURL = fileUrl,
                Amount = 100.00m, // Mock amount
                TimeStamp = DateTime.Now,
                //ApplicationId = application.Id // Link the payment to the application
            };

            await _payment.AddPaymentAsync(payment);

            // Update the application with the PaymentId
            application.PaymentId = payment.Id;
            var applicationEntity = await _application.GetApplicationByIdAsync(application.Id);
            await _application.UpdateApplicationAsync(applicationEntity.Id, application);


            // Get the name of the student
            var std = await _student.GetStudentPersonalInfoByIdAsync(applicationDto.StudentId);

            // Create notifications for employees about new application
            await _notification.AddNotificationAsync(new Notification
            {
                Message = $"Student: {std.firstName} {std.lastName} - ID: {applicationDto.StudentId} \n submitted a new application: Military Education",
                //EmployeeId = 6,
                AIDocumentId = application.Documents?.FirstOrDefault()?.Id,
                CreatedAt = DateTime.Now,
                fromStudent = true,
                StudentId = applicationDto.StudentId,
            });

            return CreatedAtRoute("StudentPersonalInfo", new { Id = applicationDto.StudentId }, application);
        }

        [HttpPost]
        [Route("trascript")]
        public async Task<IActionResult> RequestAcademicTranscript([FromForm] CreateApplicationDto applicationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (applicationDto.StudentDocument?.Length > 1048576) // Max size 1 MB in bytes
            {
                return BadRequest("File size cannot exceed 1 MB.");
            }

            // Get the file type and extension
            var fileType = GetFileType(applicationDto.StudentDocument);
            var fileExtension = GetFileExtension(fileType);

            // Generate a unique filename for student's document with extension
            var student = await _student.GetStudentPersonalInfoByIdAsync(applicationDto.StudentId);
            var studentFileName = GenerateUniqueFileName(student.firstName + '_' + student.lastName) + fileExtension;

            // Save student's uploaded document to Google Cloud Storage (if any) and handle success/failure
            string fileUrl = null;
            if (applicationDto.StudentDocument != null)
            {
                try
                {
                    fileUrl = await _googleCloud.UploadFileAsync(applicationDto.StudentDocument);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload document: {ex.Message}");
                }
            }

            // Create the application entity with details
            var application = new AIDMS.Entities.Application
            {
                Title = "Academic Transcript",
                Description = applicationDto.Description,
                Status = "Pending",
                StudentId = applicationDto.StudentId,
                SubmittedAt = DateTime.Now,
                isAccepted = false
            };

            if (applicationDto.StudentDocument != null)
            {
                application.Documents = new List<AIDocument>
                {
                    new AIDocument
                    {
                        FileName = studentFileName, // Use the generated unique filename with extension
                        FileType = fileType, // Get the actual file type
                        FilePath = fileUrl, // Use the URL from Google Cloud Storage
                        UploadedAt = DateTime.Now,
                        StudentId = applicationDto.StudentId,
                    }
                };
            }

            // Save the application entity first
            await _application.AddApplicationAsync(application);

            // Create and save a mock Payment entity linked to the application
            var payment = new Payment
            {
                DocumentURL = fileUrl,
                Amount = 100.00m, // Mock amount
                TimeStamp = DateTime.Now,
                //ApplicationId = application.Id // Link the payment to the application
            };

            await _payment.AddPaymentAsync(payment);

            // Update the application with the PaymentId
            application.PaymentId = payment.Id;
            var applicationEntity = await _application.GetApplicationByIdAsync(application.Id);
            await _application.UpdateApplicationAsync(applicationEntity.Id, application);


            // Get the name of the student
            var std = await _student.GetStudentPersonalInfoByIdAsync(applicationDto.StudentId);

            // Create notifications for employees about new application
            await _notification.AddNotificationAsync(new Notification
            {
                Message = $"Student: {std.firstName} {std.lastName} - ID: {applicationDto.StudentId} \n Requested a new application: Academic Transcript",
                //EmployeeId = 6,
                AIDocumentId = application.Documents?.FirstOrDefault()?.Id,
                CreatedAt = DateTime.Now,
                fromStudent = true,
                StudentId = applicationDto.StudentId,
            });

            return CreatedAtRoute("StudentPersonalInfo", new { Id = applicationDto.StudentId }, application);
        }

        [HttpPost]
        [Route("enrollmentproof")]
        public async Task<IActionResult> RequestEnrollmentProof([FromForm] CreateApplicationDto applicationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (applicationDto.StudentDocument?.Length > 1048576) // Max size 1 MB in bytes
            {
                return BadRequest("File size cannot exceed 1 MB.");
            }

            // Get the file type and extension
            var fileType = GetFileType(applicationDto.StudentDocument);
            var fileExtension = GetFileExtension(fileType);

            // Generate a unique filename for student's document with extension
            var student = await _student.GetStudentPersonalInfoByIdAsync(applicationDto.StudentId);
            var studentFileName = GenerateUniqueFileName(student.firstName + '_' + student.lastName) + fileExtension;

            // Save student's uploaded document to Google Cloud Storage (if any) and handle success/failure
            string fileUrl = null;
            if (applicationDto.StudentDocument != null)
            {
                try
                {
                    fileUrl = await _googleCloud.UploadFileAsync(applicationDto.StudentDocument);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload document: {ex.Message}");
                }
            }

            // Create the application entity with details
            var application = new AIDMS.Entities.Application
            {
                Title = "Enrollment Proof",
                Description = applicationDto.Description,
                Status = "Pending",
                StudentId = applicationDto.StudentId,
                SubmittedAt = DateTime.Now,
                isAccepted = false
            };

            if (applicationDto.StudentDocument != null)
            {
                application.Documents = new List<AIDocument>
                {
                    new AIDocument
                    {
                        FileName = studentFileName, // Use the generated unique filename with extension
                        FileType = fileType, // Get the actual file type
                        FilePath = fileUrl, // Use the URL from Google Cloud Storage
                        UploadedAt = DateTime.Now,
                        StudentId = applicationDto.StudentId,
                    }
                };
            }

            // Save the application entity first
            await _application.AddApplicationAsync(application);

            // Create and save a mock Payment entity linked to the application
            var payment = new Payment
            {
                DocumentURL = fileUrl,
                Amount = 100.00m, // Mock amount
                TimeStamp = DateTime.Now,
                //ApplicationId = application.Id // Link the payment to the application
            };

            await _payment.AddPaymentAsync(payment);

            // Update the application with the PaymentId
            application.PaymentId = payment.Id;
            var applicationEntity = await _application.GetApplicationByIdAsync(application.Id);
            await _application.UpdateApplicationAsync(applicationEntity.Id, application);


            // Get the name of the student
            var std = await _student.GetStudentPersonalInfoByIdAsync(applicationDto.StudentId);

            // Create notifications for employees about new application
            await _notification.AddNotificationAsync(new Notification
            {
                Message = $"Student: {std.firstName} {std.lastName} - ID: {applicationDto.StudentId} \n Requested a new application: Enrollment Proof",
                //EmployeeId = 6,
                AIDocumentId = application.Documents?.FirstOrDefault()?.Id,
                CreatedAt = DateTime.Now,
                fromStudent = true,
                StudentId = applicationDto.StudentId,
            });

            return CreatedAtRoute("StudentPersonalInfo", new { Id = applicationDto.StudentId }, application);
        }

        [HttpPut]
        [Route("settings/{studentId}")]
        public async Task<IActionResult>ChangePersonalInfo(int studentId, [FromBody] UserSettingsDto studentDto)
        {
            Student std = _context.Students.FirstOrDefault(s => s.Id == studentId);  
            if(std == null )
            {
                return BadRequest($"Student With id: {studentId} is not found!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _student.UpdateStudentAsync(studentId, studentDto);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private string GetFileExtension(string contentType)
        {
            return contentType.ToLower() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "application/pdf" => ".pdf",
                "text/plain" => ".txt",
                "application/word" => ".word",
                _ => string.Empty,
            };
        }

        //private async Task<bool> SaveStudentDocument(IFormFile studentDocument, string fileName)
        //{
        //    var folderPath = Path.Combine(@"C:\Users\AIA\Desktop\TestDocument", fileName);
        //    if (!Directory.Exists(Path.GetDirectoryName(folderPath)))
        //    {
        //        try
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(folderPath));
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error creating directory: {ex.Message}");
        //            return false;
        //        }
        //    }

        //    try
        //    {
        //        using (var stream = new FileStream(folderPath, FileMode.Create))
        //        {
        //            await studentDocument.CopyToAsync(stream);
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error saving document: {ex.Message}");
        //        return false;
        //    }
        //}

        private string GenerateUniqueFileName(string studentName)
        {
            string baseFileName = $"{studentName}_{Guid.NewGuid()}";
            if (baseFileName.Length > 45)
            {
                int allowableStudentNameLength = 45 - Guid.NewGuid().ToString().Length - 1;
                studentName = studentName.Substring(0, Math.Min(studentName.Length, allowableStudentNameLength));
                baseFileName = $"{studentName}_{Guid.NewGuid()}";
            }
            return baseFileName;
        }

        private string GetFileType(IFormFile studentDocument)
        {
            try
            {
                return studentDocument.ContentType;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting file type: {ex.Message}");
                return "Unknown";
            }
        }
    }
}
