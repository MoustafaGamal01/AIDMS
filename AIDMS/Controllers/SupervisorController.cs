using AIDMS.DTOs;
using AIDMS.Entities;
using AIDMS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIDMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SupervisorController : Controller {
    
    private readonly IStudentRepository _student;
    private readonly IApplicationRepository _application;
    private readonly INotificationRepository _notification;
    private readonly ISupervisorRepository _supervisor ;
    private readonly IRoleRepository _role;
    private readonly IEmployeeRepository _emp;
    
    public SupervisorController(IApplicationRepository application,INotificationRepository notification,
        ISupervisorRepository supervisor,IRoleRepository role,IEmployeeRepository emp)
    {
        _emp = emp;
        _application = application;
        _notification = notification;
        _supervisor = supervisor;
        _role = role;
    }
    
    [HttpGet]
    [Route("settings/{supervisorId:int}")]
    public async Task<IActionResult> GetSupervisorIdSetting(int supervisorId)
    {
        var supervisor = await _supervisor.GetSupervisorByIdAsync(supervisorId);
        if (supervisor == null)
        {
            return NotFound("Supervisor not found");
        }
        if (ModelState.IsValid)
        {
            UserSettingsDto supervisorSettingsDto = new UserSettingsDto
            {
                userName = supervisor.userName,
                email = supervisor.Email,
                Phone = supervisor.phoneNumber,
                password = supervisor.Password,
                profilePicture = supervisor.supervisorPicture
            };
            return Ok(supervisorSettingsDto);
        }
        return BadRequest();
    }
    
}