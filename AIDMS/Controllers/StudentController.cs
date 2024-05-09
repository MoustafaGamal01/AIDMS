using AIDMS.DTOs;
using AIDMS.Entities;
using AIDMS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIDMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _student;

        public StudentController(IStudentRepository student)
        {
            this._student = student;
        }

        [HttpGet]
        [Route("{Id:int}", Name = "StudentPersonalInfo")]
        public IActionResult GetStudentPersonalInfoById(int Id)
        {
            Student std = _student.GetStudentPersonalInfoByIdAsync(Id).GetAwaiter().GetResult();
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

        [HttpGet]
        [Route("")]
        public IActionResult GetAllStudentsPersonalInfo()
        {
            List<Student> stds = _student.GetAllStudentsPersonalInfoAsync().GetAwaiter().GetResult();
            List<StudentDto> studentsDto = new List<StudentDto>();
            foreach (var std in stds)
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
                studentsDto.Add(studentDto);
            }
            return Ok(studentsDto);
        }

        [HttpPost]
        public IActionResult AddٍStudent(Student student)
        {
            if (ModelState.IsValid)
            {
                _student.AddStudentAsync(student).GetAwaiter().GetResult();
                string url = Url.Link("StudentPersonalInfo", new { student.Id });
                return Created(url, student);
            }
            return BadRequest();
        }


    }
}
