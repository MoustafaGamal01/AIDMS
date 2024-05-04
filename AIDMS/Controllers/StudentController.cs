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
        public IActionResult Get()
        {
            List<Student> stds = _student.GetAllStudentsAsync().GetAwaiter().GetResult();
            return Ok(stds);
        }

        [HttpPost]
        public IActionResult Add(Student student)
        {
            _student.AddStudentAsync(student).GetAwaiter().GetResult();
            return Ok();
        }
    }
}
