﻿using AIDMS.DTOs;
using AIDMS.Entities;
using AIDMS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using MimeKit;
using static System.Net.Mime.MediaTypeNames;

namespace AIDMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly AIDMSContextClass _context;
        private readonly IStudentRepository _student;
        private readonly IApplicationRepository _application;
        private readonly INotificationRepository _notification;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IRoleRepository _role;
        private readonly IDocumentRepository _document;

        public StudentController(AIDMSContextClass context, IStudentRepository student, IApplicationRepository application,
            INotificationRepository notification, IWebHostEnvironment hostingEnvironment, IRoleRepository role, IDocumentRepository document)
        {
            this._context = context;
            this._student = student;
            this._application = application;
            this._notification = notification;
            this._hostingEnvironment = hostingEnvironment;
            this._role = role;
            this._document = document;
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

                //var docs = _document.GetAllDocumentsByStudentIdAsync(id);

                studentDto.studentDocuments = std.Documents;
                studentDto.studentPicture = std.studentPicture;
                return Ok(studentDto);
            }
            return BadRequest();
        }

        
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

            var notificationsListDto = notifications.Select(n => new StudentNotificationDto
            {
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
                userSettingsDto.email = student.Email;
                userSettingsDto.Phone = student.PhoneNumber;
                userSettingsDto.password = student.Password;
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
            }).ToList();

            return Ok(applicationsDto);
        }

        [HttpGet]
        [Route("reviewed/{studentId:int}")]
        public async Task<IActionResult> GetReviewedApplicationsByStudentIdAsync(int studentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var applications = await _application.GetAllReviewedApplicationsByStudentIdAsync(studentId);

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

        [HttpPost]
        [Route("applications")]
        public async Task<IActionResult> SubmitApplication([FromForm] CreateApplicationDto applicationDto)
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
            var studentFileName = GenerateUniqueFileName(student.firstName+'_'+student.lastName) + fileExtension; //GenerateUniqueFileName(student.firstName+'_'+student.lastName) + fileExtension;

            // Save student's uploaded document (if any) and handle success/failure
            bool documentSaved = false;
            if (applicationDto.StudentDocument != null)
            {
                documentSaved = await SaveStudentDocument(applicationDto.StudentDocument, studentFileName);
                if (!documentSaved)
                {
                    return BadRequest("Failed to save uploaded document.");
                }
            }

            // Create the application entity with details
            var application = new AIDMS.Entities.Application
            {
                Title = applicationDto.Title,
                Description = applicationDto.Description,
                Status = "Pending",
                EmployeeId = 6, // Assign to designated employee
                StudentId = applicationDto.StudentId,
                SubmittedAt = DateTime.Now
            };

            string FileURL = documentSaved ? Path.Combine(@"C:\Users\AIA\Desktop\TestDocument", studentFileName) : null;
            if (applicationDto.StudentDocument != null)
            {
                application.Documents = new List<AIDocument>();
                application.Documents.Add(new AIDocument
                {
                    FileName = studentFileName, // Use the generated unique filename with extension
                    FileType = fileType, // Get the actual file type
                    FilePath = FileURL,
                    UploadedAt = DateTime.Now
                    //StudentId = applicationDto.StudentId,
                });
            }

            // Save the application entity first
            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();

            // Create and save a mock Payment entity linked to the application
            var payment = new Payment
            {
                DocumentURL = FileURL,
                Amount = 100.00m, // Mock amount
                TimeStamp = DateTime.Now,
                //ApplicationId = application.Id // Link the payment to the application
            };

            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();

            // Update the application with the PaymentId
            application.PaymentId = payment.Id;
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();

            // Get the name of the student
            var std = await _student.GetStudentPersonalInfoByIdAsync(applicationDto.StudentId);

            // Create notifications for employees about new application
            
            await _notification.AddNotificationAsync(new Notification
            {
                Message = $"Student: {std.firstName + ' ' + std.lastName} - ID: {applicationDto.StudentId} \n  submitted a new application: {applicationDto.Title}",
                EmployeeId = 4,
                AIDocumentId = application.Documents?.FirstOrDefault()?.Id,
            });


            return CreatedAtRoute("StudentPersonalInfo", new { Id = applicationDto.StudentId }, application);
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

        private async Task<bool> SaveStudentDocument(IFormFile studentDocument, string fileName)
        {
            var folderPath = Path.Combine(@"C:\Users\AIA\Desktop\TestDocument", fileName);
            if (!Directory.Exists(Path.GetDirectoryName(folderPath)))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(folderPath));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating directory: {ex.Message}");
                    return false;
                }
            }

            try
            {
                using (var stream = new FileStream(folderPath, FileMode.Create))
                {
                    await studentDocument.CopyToAsync(stream);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving document: {ex.Message}");
                return false;
            }
        }

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

        //[HttpPost]
        //[Route("applications")]
        //public async Task<IActionResult> MatrialsRegisteration([FromForm] CreateApplicationDto applicationDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (applicationDto.StudentDocument?.Length > 1048576) // Max size 1 MB in bytes
        //    {
        //        return BadRequest("File size cannot exceed 1 MB.");
        //    }

        //    // Get the file type and extension
        //    var fileType = GetFileType(applicationDto.StudentDocument);
        //    var fileExtension = GetFileExtension(fileType);

        //    // Generate a unique filename for student's document with extension
        //    var studentFileName = GenerateUniqueFileName(applicationDto.StudentId) + fileExtension;

        //    // Save student's uploaded document (if any) and handle success/failure
        //    bool documentSaved = false;
        //    if (applicationDto.StudentDocument != null)
        //    {
        //        documentSaved = await SaveStudentDocument(applicationDto.StudentDocument, studentFileName);
        //        if (!documentSaved)
        //        {
        //            return BadRequest("Failed to save uploaded document.");
        //        }
        //    }

        //    // Create the application entity with details
        //    var application = new AIDMS.Entities.Application
        //    {
        //        Title = applicationDto.Title,
        //        Description = applicationDto.Description,
        //        Status = "Pending",
        //        EmployeeId = 6, // Assign to designated employee
        //        StudentId = applicationDto.StudentId,
        //        SubmittedAt = DateTime.Now
        //    };

        //    string FileURL = documentSaved ? Path.Combine(@"C:\Users\AIA\Desktop\TestDocument", studentFileName) : null;
        //    if (applicationDto.StudentDocument != null)
        //    {
        //        application.Documents = new List<AIDocument>();
        //        application.Documents.Add(new AIDocument
        //        {
        //            FileName = studentFileName, // Use the generated unique filename with extension
        //            FileType = fileType, // Get the actual file type
        //            FilePath = FileURL,
        //            //StudentId = applicationDto.StudentId,
        //        });
        //    }

        //    // Save the application entity first
        //    await _context.Applications.AddAsync(application);
        //    await _context.SaveChangesAsync();

        //    // Create and save a mock Payment entity linked to the application
        //    var payment = new Payment
        //    {
        //        DocumentURL = FileURL,
        //        Amount = 100.00m, // Mock amount
        //        TimeStamp = DateTime.Now,
        //        //ApplicationId = application.Id // Link the payment to the application
        //    };

        //    await _context.Payments.AddAsync(payment);
        //    await _context.SaveChangesAsync();

        //    // Update the application with the PaymentId
        //    application.PaymentId = payment.Id;
        //    _context.Applications.Update(application);
        //    await _context.SaveChangesAsync();

        //    // Get the name of the student
        //    var std = await _student.GetStudentPersonalInfoByIdAsync(applicationDto.StudentId);

        //    // Create notifications for employees about new application

        //    await _notification.AddNotificationAsync(new Notification
        //    {
        //        Message = $"Student: {std.firstName + ' ' + std.lastName} - ID: {applicationDto.StudentId} \n  submitted a new application: {applicationDto.Title}",
        //        EmployeeId = 4,
        //        AIDocumentId = application.Documents?.FirstOrDefault()?.Id,
        //    });


        //    return CreatedAtRoute("StudentPersonalInfo", new { Id = applicationDto.StudentId }, application);
        //}

    }
}
