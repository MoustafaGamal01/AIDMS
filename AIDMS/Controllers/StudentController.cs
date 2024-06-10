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
        public IActionResult GetStudentPersonalInfoById(int Id)
        {
            Student std = _student.GetStudentPersonalInfoByIdAsync(Id).GetAwaiter().GetResult();
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

        //[HttpGet]
        //[Route("")]
        //public IActionResult GetAllStudentsPersonalInfo()
        //{
        //    List<Student> stds = _student.GetAllStudentsPersonalInfoAsync().GetAwaiter().GetResult();
        //    List<StudentDto> studentsDto = new List<StudentDto>();
        //    if (stds == null)
        //    {
        //        return NotFound("No Students Available");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        foreach (var std in stds)
        //        {
        //            StudentDto studentDto = new StudentDto();
        //            studentDto.Id = std.Id;
        //            studentDto.GPA = std.GPA;
        //            studentDto.TotalPassedHours = std.TotalPassedHours;
        //            studentDto.Level = std.Level;
        //            studentDto.firstName = std.firstName;
        //            studentDto.lastName = std.lastName;
        //            studentDto.studentDepartment = std.Department.Name;
        //            studentDto.studentDocuments = std.Documents;
        //            studentDto.studentPicture = std.studentPicture;
        //            studentsDto.Add(studentDto);
        //        }
        //        return Ok(studentsDto);
        //    }
        //    return BadRequest();
        //}

        //[HttpPost]
        //[Route("")]
        //public IActionResult AddStudent(Student student)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _student.AddStudentAsync(student).GetAwaiter().GetResult();
        //        string url = Url.Link("StudentPersonalInfo", new { student.Id });
        //        return Created(url, student);
        //    }
        //    return BadRequest();
        //}

        [HttpGet]
        [Route("pending/{studentId:int}")]
        public IActionResult GetPendingApplicationsByStudentIdAsync(int studentId)
        {
            var studentApplications = _application.GetAllPendingApplicationsByStudentIdAsync(studentId).GetAwaiter().GetResult();

            if (ModelState.IsValid)
            {
                return Ok(studentApplications);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("reviewed/{studentId:int}")]
        public IActionResult GetReviewedApplicationsByStudentIdAsync(int studentId)
        {
            var studentApplications = _application.GetAllReviewedApplicationsByStudentIdAsync(studentId).GetAwaiter().GetResult();

            if (ModelState.IsValid)
            {
                return Ok(studentApplications);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("notifications/{studentId:int}")]
        public IActionResult GetStudentNotifications(int studentId)
        {
            var notifications = _notification.GetAllNotificationsByStudentIdAsync(studentId).GetAwaiter().GetResult();
            List<StudentNotificationDto> notificationsListDto = new List<StudentNotificationDto>();
            if (notifications == null || (!ModelState.IsValid))
            {
                return BadRequest();
            }

            foreach (var notification in notifications)
            {
                StudentNotificationDto notificationDto = new StudentNotificationDto();
                notificationDto.Message = notification.Message;
                notificationDto.CreatedAt = notification.CreatedAt.ToString();
                notificationDto.EmployeeName = notification.Employee.firstName + notification.Employee.lastName;
                notificationsListDto.Add(notificationDto);
            }
            return Ok(notificationsListDto);
        }
    }
}
