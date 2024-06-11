using AIDMS.DTOs;
using AIDMS.Entities;
using AIDMS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

        public StudentController(AIDMSContextClass context, IStudentRepository student, IApplicationRepository application,
            INotificationRepository notification)
        {
            this._context = context;
            this._student = student;
            this._application = application;
            this._notification = notification;
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

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromQuery] int studentId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File is not selected or is empty" });

            // Save file locally
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine("uploads", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save document details in the database
            var document = new AIDocument
            {
                FileName = fileName,
                FileType = file.ContentType,
                FilePath = filePath,
                UploadedAt = DateTime.UtcNow,
                StudentId = studentId
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            // Find an employee with RoleId = 2 (this logic may vary depending on your requirements)
            var employee = _context.Employees.FirstOrDefault(e => e.RoleId == 2);
            if (employee == null)
            {
                return BadRequest(new { message = "No employee with RoleId = 2 found" });
            }

            // Create a notification
            var notification = new Notification
            {
                Message = $"A new document has been uploaded by student with ID {studentId}.",
                AIDocumentId = document.Id,
                EmployeeId = employee.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "File uploaded successfully and notification sent", filePath });
        }



    }
}
