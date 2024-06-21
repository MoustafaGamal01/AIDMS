using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace AIDMS.Controllers
{
    // Test Controller
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class Test : ControllerBase
    {
        [Authorize(Roles = "Student")]
        [HttpGet]
        [Route("Student")]
        public IActionResult Test1()
        {
            return Ok("Hello Student!");
        }

         [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin")]
        public IActionResult Test2()
        {
            return Ok("Hello Admin!");
        }

      [Authorize(Roles = "Affairs Officer")]
        [HttpGet]
        [Route("Affairs-Officer")]
        public IActionResult Test3()
        {
            return Ok("Hello Affairs Officer!");
        }

     [Authorize(Roles = "Academic Supervisor")]
        [HttpGet]
        [Route("Supervisor")]
        public IActionResult Test4()
        {
            return Ok("Hello Supervisor!");
        }
    }
}
