using AIDMS.DTOs;
using AIDMS.Entities;
using AIDMS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIDMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApplicationController : Controller {
    
    private readonly IApplicationRepository _application;
    private readonly INotificationRepository _notification;
    private readonly IStudentRepository _student;
    public ApplicationController(IApplicationRepository application,INotificationRepository notification
    ,IStudentRepository student)
    {
        _application = application;
        _notification = notification;
        _student = student;
    }
    
#region Admin Applications
    
    [HttpGet]
    [Route("admin")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationBaseInfoDto>))]
    public async Task<IEnumerable<ApplicationBaseInfoDto>> GetAllApplicationsBaseInfo()
    {
        var Applications = await _application.GetAllApplicationsAsync();
        var applicationBaseInfo = Applications.Select(app => new ApplicationBaseInfoDto
        {
            Id = app.Id,
            Title = app.Title,
            Date = app.SubmittedAt,
            DecisionDate = app.DecisionDate,
            Status = app.Status
        });
        return applicationBaseInfo;
    }    

#endregion    

#region Get Application Request for the employee

    [HttpGet]
    [Route("pending/employee")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationRequestDto>))]
    [ProducesResponseType(400)]
    public async Task<IEnumerable<ApplicationRequestDto>> GetPendingApplicationsExceptMaterial()
    {
        
        var Applications = await _application.GetAllPendingApplicationsAsync();
        var applicationRequestDto = Applications
            .Where(application=>application.Title.ToUpper()!="material".ToUpper()&&
                                application.Title.ToUpper()!="registeration".ToUpper())
            .Select(app => new ApplicationRequestDto
        {
            Id = app.Id,
            Name = app.Title,
            Date = app.SubmittedAt,
            From = $"{app.Student.firstName} {app.Student.lastName}"
        });
        
        return applicationRequestDto;
    }
    

    [HttpGet]
    [Route("archived/employee")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationArchivedDto>))]
    public async Task<IEnumerable<ApplicationArchivedDto>> GetArchivedApplicationsExceptMaterial()
    {
        var Applications = await _application.GetAllArchivedApplicationsAsync();
        var applicationArchivedDto = Applications
            .Where(application=>application.Title.ToUpper()!="Material".ToUpper()&&
                                application.Title.ToUpper()!="registeration".ToUpper())
            .Select(app => new ApplicationArchivedDto
            {
                Id = app.Id,
                Name = app.Title,
                Date = app.SubmittedAt,
                From = $"{app.Student.firstName} {app.Student.lastName}",
                IsAccepted = app.isAccepted
            });
        return applicationArchivedDto;
    }    

    
#endregion

#region Get Application Request for the employee by search
   
    [HttpGet]
    [Route("pending/employee/{studentName}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationRequestDto>))]
    public async Task<IEnumerable<ApplicationRequestDto>> GetSearchInPendingApplicationsExceptMaterial(string studentName)
    {
        var Applications = await GetPendingApplicationsExceptMaterial();
        var applicationRequestDto = Applications
            .Where(app => (app.From.Replace(" ", "").ToUpper())
                .Contains(studentName.Replace(" ", "").ToUpper()));
        return applicationRequestDto;
    }
        
    [HttpGet]
    [Route("archived/employee/{studentName}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationArchivedDto>))]
    public async Task<IEnumerable<ApplicationArchivedDto>> GetSearchInArchivedApplicationsExceptMaterial(string studentName)
    {
        var Applications = await GetArchivedApplicationsExceptMaterial();
        var applicationArchivedDto = Applications
                .Where(app => (app.From.Replace(" ", "").ToUpper())
                    .Contains(studentName.Replace(" ", "").ToUpper()));
        return applicationArchivedDto;
    }
    
    
    
    
#endregion

#region Registeration

    [HttpGet]
    [Route("pending/registeration")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<RegisterationDto>))]
    public async Task<IEnumerable<RegisterationDto>> GetPendingRegisteration()
    {
        var Applications = await _application.GetAllPendingApplicationsAsync();
        var registerationDto = Applications
            .Where(application=>application.Title.ToUpper() == "registeration".ToUpper())
            .Select(app => new RegisterationDto
            {
                Id = app.Id,
                Date = app.SubmittedAt,
                Name = $"{app.Student.firstName} {app.Student.lastName}",
            });
        return registerationDto;
    }
        
        
    [HttpGet]
    [Route("archived/registeration")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<RegisterationDto>))]
    public async Task<IEnumerable<RegisterationDto>> GetArchivedRegisteration()
    {
        var Applications = await _application.GetAllArchivedApplicationsAsync();
        var registerationArchivedDto = Applications
            .Where(application=>application.Title.ToUpper() != "registeration".ToUpper())
            .Select(app => new RegisterationDto
            {
                Id = app.Id,
                Date = app.SubmittedAt,
                Name = $"{app.Student.firstName} {app.Student.lastName}",
            });
        return registerationArchivedDto;
    }  


#endregion

#region Accept & Decline Registeration
    [HttpDelete("decline/registeration/{appId}")]
    [ProducesResponseType(400)]
    public async Task<IActionResult> deleteRegisterationApplicationStatus(int appId)
    {
        var application = await _application.GetApplicationByIdAsync(appId);
        int studentId = (int)application.StudentId;
        
        bool? affected = await _application.DeleteApplicationAsync(appId);
        if (affected == null)
        {
            return BadRequest();
        }
        affected = await _student.DeleteStudentAsync(studentId);
        if (affected == null)
        {
            return BadRequest();
        }
        
        return Ok();
    }
    
#endregion

#region Accept & Decline

    [HttpPut("accept/{empId}/{appId}")]
    [ProducesResponseType(400)]

    public async Task<IActionResult> UpdateAcceptAppStatus( int empId,int appId)
    {
        var application = await _application.GetApplicationByIdAsync(appId);
        
        application.isAccepted = true;
        application.Status = "archived";
        application.EmployeeId = empId;      
        application.DecisionDate=DateTime.Now;
        bool? updated = await _application.UpdateApplicationAsync(appId, application);
        if (updated == null)
        {
            return BadRequest();
        }

        if (application.Title == "Military Education")
        {
            await _student.UpdateStudentMilitaryAsync((int)application.StudentId);
        }
        
        var added = await _notification.AddNotificationAsync(new Notification
        {
            Message = $"""
                       Dear {application.Student},
                       I am pleased to inform you that your recent {application.Title} request has been accepted. 
                       Thank you for your attention to this matter.
                       Best regards,
                       """,
            CreatedAt = DateTime.Now,
            StudentId=application.StudentId,
            AIDocumentId = application.Documents?.FirstOrDefault()?.Id,
            EmployeeId = empId,
            fromStudent = false
        });
        
        if (added == true)
        {
            return Ok();
        }
        
        return BadRequest();
    }
    
    [HttpPut("decline/{empId}/{appId}")]
    [ProducesResponseType(400)]

    public async Task<IActionResult> UpdateDeclineAppStatus( int empId,int appId)
    {
        var application = await _application.GetApplicationByIdAsync(appId);
        
        application.isAccepted = false;
        application.Status = "archived";
        application.EmployeeId = empId;      
        application.DecisionDate=DateTime.Now;
        bool? updated = await _application.UpdateApplicationAsync(appId, application);
        if (updated == null)
        {
            return BadRequest();
        }
        
        var added = await _notification.AddNotificationAsync(new Notification
        {
            Message = $"""
                       Dear {application.Student},
                       I apologize for declining your recent {application.Title} request.
                       Thank you for your attention to this matter.
                       Best regards,
                       """,
            CreatedAt = DateTime.Now,
            StudentId=application.StudentId,
            AIDocumentId = application.Documents?.FirstOrDefault()?.Id,
            EmployeeId = empId,
            fromStudent = false
        });
        
        if (added == true)
        {
            return Ok();
        }
        
        return BadRequest();
    }


#endregion
    
#region Get Application Request for the supervisor
    
    [HttpGet]
    [Route("pending/supervisor/{empId:int}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationRequestDto>))]
    public async Task<IEnumerable<ApplicationRequestDto>> GetPendingApplicationsWithMaterial(int empId)
    {
        var Applications = await _application.GetAllPendingApplicationsByEmployeeIdAsync(empId);
        var applicationRequestDto = Applications
            .Where(application=>application.Title.ToUpper()=="Material".ToUpper())
            .Select(app => new ApplicationRequestDto
        {
            Id = app.Id,
            Name = app.Title,
            Date = app.SubmittedAt,
            From = $"{app.Student.firstName} {app.Student.lastName}"
        });
        return applicationRequestDto;
    }
    
  
    [HttpGet]
    [Route("archived/supervisor/{empId:int}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationArchivedDto>))]
    public async Task<IEnumerable<ApplicationArchivedDto>> GetArchivedApplicationsWithMaterial(int empId)
    {
        var Applications = await _application.GetAllArchivedApplicationsByEmployeeIdAsync(empId);
        var applicationArchivedDto = Applications
            .Where(application=>application.Title.ToUpper()=="Material".ToUpper())
            .Select(app => new ApplicationArchivedDto
            {
                Id = app.Id,
                Name = app.Title,
                Date = app.SubmittedAt,
                From = $"{app.Student.firstName} {app.Student.lastName}",
                IsAccepted = app.isAccepted
            });
        return applicationArchivedDto;
    }

#endregion

#region Get Application Request for the supervisor by search

    [HttpGet]
    [Route("pending/supervisor/{empId:int}/{studentName}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationRequestDto>))]
    public async Task<IEnumerable<ApplicationRequestDto>> GetSearchInPendingApplicationsWithMaterial(int empId,string studentName)
    {
        var Applications = await GetPendingApplicationsWithMaterial(empId);
        var applicationRequestDto = Applications
                .Where(app => (app.From.Replace(" ", "").ToUpper())
                    .Contains(studentName.Replace(" ", "").ToUpper()));
        return applicationRequestDto;
    }
    
        
    [HttpGet]
    [Route("archived/supervisor/{empId:int}/{studentName}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationArchivedDto>))]
    public async Task<IEnumerable<ApplicationArchivedDto>> GetSearchArchivedApplicationsWithMaterial(int empId,string studentName)
    {
        var Applications = await GetArchivedApplicationsWithMaterial(empId);
        var applicationArchivedDto = Applications
                .Where(app => (app.From.Replace(" ", "").ToUpper())
                    .Contains(studentName.Replace(" ", "").ToUpper()));
        return applicationArchivedDto;
    }

#endregion

}
