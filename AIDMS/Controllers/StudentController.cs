using AIDMS.DTOs;
using AIDMS.Entities;
using AIDMS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIDMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _student;
        private readonly IApplicationRepository _application;
        private readonly INotificationRepository _notification;

        public StudentController(IStudentRepository student, IApplicationRepository application,
            INotificationRepository notification)
        {
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
            var studentApplications = await _application.GetAllPendingApplicationsByStudentIdAsync(studentId);

            if (ModelState.IsValid)
            {
                return Ok(studentApplications);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("reviewed/{studentId:int}")]
        public async Task<IActionResult> GetReviewedApplicationsByStudentIdAsync(int studentId)
        {
            var studentApplications = await _application.GetAllReviewedApplicationsByStudentIdAsync(studentId);

            if (ModelState.IsValid)
            {
                return Ok(studentApplications);
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
    }
}
