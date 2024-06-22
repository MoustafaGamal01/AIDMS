using AIDMS.DTOs;
using AIDMS.Entities;
using AIDMS.Repositories;
using AIDMS.Security_Entities;
using Google.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIDMS.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class EmployeeController : Controller
{
    private readonly AIDMSContextClass _context;
    private readonly IEmployeeRepository _emp;
    private readonly INotificationRepository _notification;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public EmployeeController(AIDMSContextClass context,IEmployeeRepository emp, INotificationRepository notification, UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
        this._context = context;
        _emp = emp;
        this._notification = notification;
        this._userManager = userManager;
        this._signInManager = signInManager;
        this._roleManager = roleManager;
    }
    [Authorize(Roles = "Affairs Officer, Academic Supervisor")]
    [HttpGet]
    [Route("notifications/{empId:int}")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetAllNotificationsByEmployeeId(int empId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var notifications = await _notification.GetAllNotificationsByEmployeeIdAsync(empId);


        if (notifications == null)
        {
            return NotFound();
        }

        var EmpNotificationDto = notifications.Select(n => new UserNotificationDto
        {
            userPicture = n.Student.studentPicture,
            userFirstName = n.Student.firstName,
            userLastName = n.Student.lastName,
            Message = n.Message,
            CreatedAt = n.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        });


        return Ok(EmpNotificationDto);
    }

    [Authorize(Roles = "Admin, Student")]
    [HttpGet]
    [Route("GetAllSupervisors")]
    public async Task<IActionResult> GetAllSupervisors()
    {
        // Assuming "Supervisor" is the role name for supervisors
        string supervisorRoleName = "Academic Supervisor";

        // Find the role
        var role = await _roleManager.FindByNameAsync(supervisorRoleName);
        if (role == null)
        {
            return NotFound($"Role '{supervisorRoleName}' not found.");
        }

        // Get the list of users in the Supervisor role
        var usersInRole = await _userManager.GetUsersInRoleAsync(supervisorRoleName);

        if (usersInRole == null || !usersInRole.Any())
        {
            return NotFound("No supervisors found.");
        }
        var supervisors = usersInRole.Select(user => new
        {
            Username = user.UserName,
            Email = user.Email
        });

        return Ok(supervisors);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("GetAllUsersWithRoles")]
    public async Task<IActionResult> GetAllUsersWithRoles()
    {
        var users = _userManager.Users.ToList();
        var userRoles = new List<UserWithRolesDto>();
        var query = from emp in _context.Employees
                    join user in _userManager.Users
                    on emp.userName equals user.UserName
                    join userRole in _context.UserRoles
                    on user.Id equals userRole.UserId
                    join role in _context.Roles
                    on userRole.RoleId equals role.Id
                    select new
                    {
                        EmployeeId = emp.Id,
                        EmployeeName = $"{emp.firstName} {emp.lastName}",
                        RoleName = role.Name
                    };
        var result = await query.ToListAsync();
        return Ok(result);
    }

    [Authorize(Roles=("Affairs Officer, Academic Supervisor, Admin"))]
    [HttpGet]
    [Route("settings/{employeeId:int}")]
    public async Task<IActionResult> GetEmplyeesSetting(int employeeId)
    {
        var employee = await _emp.GetEmployeeByIdAsync(employeeId);
        if (employee == null)
        {
            return NotFound("Employee not found");
        }
        if (ModelState.IsValid)
        {
            UserSettingsDto employeeSettingsDto = new UserSettingsDto
            {
                userName = employee.userName,
                Phone = employee.phoneNumber,
                profilePicture = employee.employeePicture
            };
            return Ok(employeeSettingsDto);
        }
        return BadRequest();
    }


    private int CalculateAge(DateTime dateOfBirth)
    {
        DateTime now = DateTime.Now;
        int age = now.Year - dateOfBirth.Year;
        if (now < dateOfBirth.AddYears(age))
            age--;
        return age;
    }

    [Authorize(Roles =("Admin"))]
    [HttpPost]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRegestrationDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

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
            NationalId = model.nationalId,
        };

        var result = await _userManager.CreateAsync(applicationUser, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Create Cookie
        await _userManager.AddToRoleAsync(applicationUser, model.role);
        await _signInManager.SignInAsync(applicationUser, isPersistent: false);

        // Step 4: Create Student Record without storing the plain password
        var student = new Employee
        {
            firstName = model.firstName, // From Google Vision Model
            lastName = model.lastName, // From Google Vision Model
            phoneNumber = model.PhoneNumber,
            userName = model.Username,
            dateOfBirth = model.dateOfBirth,
            IsMale = model.isMale,
            employeePicture = model.employeePicture,
            Age = CalculateAge(model.dateOfBirth)
        };

        bool? ok = await _emp.AddEmployeeAsync(student);
        if (ok == false)
            return BadRequest("Error, Please Check The Info Again!");

        return Ok(new { Message = "Successful Registration" });
    }

    [Authorize(Roles = ("Admin"))]
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Delete(int id)
    {
        // Delete from Employee table
        var emp = await _emp.GetEmployeeByIdAsync(id);
        if (emp == null)
        {
            return BadRequest($"Can't find Employee with id: {id}.");
        }
        var user = await _userManager.FindByNameAsync(emp.userName);
        if (user != null)
        {
            // Delete the user from UserManager
            IdentityResult result = await _userManager.DeleteAsync(user);
            bool? ok = await _emp.DeleteEmployeeAsync(id);
            if (!result.Succeeded)
            {
                return BadRequest($"Failed to delete the user associated with employee id: {id}.");
            }
        }
        return Ok("Employee Deleted"); 
    }

    [Authorize(Roles = ("Admin, Affairs Officer, Academic Supervisor"))]
    [HttpPut("{id}")]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateEmployee( int id,[FromBody] UpdateEmployeeDto updateEmp)
    {
        var emp = await _emp.GetEmployeeByIdAsync(id);
        if (emp == null)
        {
            return BadRequest($"Employee With id: {id} is not found!");
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            await _emp.UpdateEmployeeAsync(id, updateEmp);
            return Ok(updateEmp);
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
}
