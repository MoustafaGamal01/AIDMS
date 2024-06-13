using AIDMS.DTOs;
using AIDMS.Entities;
using AIDMS.Repositories;
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

    public EmployeeController(IEmployeeRepository emp, IRoleRepository role)
    {
        _emp = emp;
        _role = role;
    }
    
    [HttpGet]
    [Route("admin")]
    // [ProducesResponseType(200, Type = typeof(ActionResult<BaseEmployeeDto>))]
    public async Task<ActionResult<IEnumerable<BaseEmployeeDto>>> GetAllEmplyeesBaseInfo()
    {
        var Employees = await _emp.GetAllEmployeesAndRoleAsync();
        var employeesBaseInfo = Employees.Select(e => new BaseEmployeeDto
        {
            Id = e.Id,
            Name = $"{e.firstName} {e.lastName}",
            roleName = e.Role != null ? e.Role.Name : ""
        });
        return Ok(employeesBaseInfo);
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