﻿using AIDMS.DTOs;
using AIDMS.Entities;
using AIDMS.Repositories;
using Google.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIDMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : Controller
{
    private readonly IEmployeeRepository _emp;
    private readonly IRoleRepository _role;
    private readonly INotificationRepository _notification;

    public EmployeeController(IEmployeeRepository emp, IRoleRepository role, INotificationRepository notification)
    {
        _emp = emp;
        _role = role;
        this._notification = notification;
    }
    
    [HttpGet]
    [Route("admin/{adminId:int}")]
    // [ProducesResponseType(200, Type = typeof(ActionResult<BaseEmployeeDto>))]
    public async Task<ActionResult<IEnumerable<BaseEmployeeDto>>> GetAllEmplyeesBaseInfo(int adminId)
    {
        var Employees = await _emp.GetAllEmployeesAndRoleAsync();
        var employeesBaseInfo = Employe
        .Where(em => em.Id != adminId).Select(e => new BaseEmployeeDto
        {
            Id = e.Id,
            Name = $"{e.firstName} {e.lastName}",
            roleName = e.Role != null ? e.Role.Name : ""
        });
        return Ok(employeesBaseInfo);
    }

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

    [HttpGet]
    [Route("supervisors")]
    public async Task<ActionResult<IEnumerable<GetAllSupervisorsDto>>> GetAllSupervisors()
    {
        var supervisors = await _emp.GetAllSupervisorsAsync();
        var supervisorsDto = supervisors.Select(e => new GetAllSupervisorsDto
        {
            Id = e.Id,
            Name = $"{e.firstName} {e.lastName}",
        });
        return Ok(supervisorsDto);
    }

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
                email = employee.Email,
                Phone = employee.phoneNumber,
                password = employee.Password,
                profilePicture = employee.employeePicture
            };
            return Ok(employeeSettingsDto);
        }
        return BadRequest();
    }
    
    /////////////////
    // [HttpGet]
    // public async Task<IEnumerable<Employee>> GetAllEmplyeesBaseInfo()
    // {
    //     var Employees = await _emp.GetAllEmployeesAsync();
    //     return Employees;
    // }
    //////////////
    
    [HttpPost]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateEmployee([FromBody] Employee em)
    {
        if (em == null)
        {
            return BadRequest("invalid employee data");
        }
        
        var addedEmployee = await _emp.AddEmployeeAsync(em);
        if (addedEmployee == null)
        {
            return BadRequest("failed to add the employee");
        }

        return Ok(em);
    }

    
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Delete(int id)
    {
        bool? deleted = await _emp.DeleteEmployeeAsync(id);
        if (deleted.HasValue && deleted.Value) // Short circuit AND.
        {
            return new NoContentResult(); // 204 No content.
        }
        else
        {
            return BadRequest( // 400 Bad request.
                $"failed to delete the employee with id :{id}.");
        }
    }


    [HttpPut("{id}")]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateEmployee( int id,[FromBody] UpdateEmployeeDto updateEmp)
    {
        var updated = await _emp.UpdateEmployeeBaseInfoAsync(id, updateEmp);
        if (updated == null)
        {
            return BadRequest("Failed to update");
        }

        var em = await _emp.GetEmployeeByIdAsync(id);
        return Ok(em);
    }
}
