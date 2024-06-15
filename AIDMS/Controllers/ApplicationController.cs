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
    public ApplicationController(IApplicationRepository application,INotificationRepository notification)
    {
        _application = application;
        _notification = notification;
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
    public async Task<IEnumerable<ApplicationRequestDto>> GetPendingApplicationsExceptMaterial()
    {
        var Applications = await _application.GetAllPendingApplicationsAsync();
        var applicationRequestDto = Applications
            .Where(application=>application.Title.ToUpper()!="Material".ToUpper())
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
        var Applications = await _application.GetAllArchivedApplicationsWithStudentRelatedAsync();
        var applicationArchivedDto = Applications
            .Where(application=>application.Title.ToUpper()!="Material".ToUpper())
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
            .Where(app => app.From.Replace(" ","").ToUpper() == studentName.Replace(" ","").ToUpper());
        return applicationRequestDto;
    }
        
   
        
    [HttpGet]
    [Route("archived/employee/{studentName}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationArchivedDto>))]
    public async Task<IEnumerable<ApplicationArchivedDto>> GetSearchInArchivedApplicationsExceptMaterial(string studentName)
    {
        var Applications = await GetArchivedApplicationsExceptMaterial();
        var applicationArchivedDto = Applications
            .Where(app => app.From.Replace(" ","").ToUpper() == studentName.Replace(" ","").ToUpper());
        return applicationArchivedDto;
    }
#endregion


    [HttpPut("{empId}/{appId}")]
    [ProducesResponseType(400)]

    public async Task<IActionResult> UpdateAppStatus( int empId,int appId,[FromBody] bool isAccepted)
    {
        var application = await _application.GetApplicationByIdAsync(appId);
        
        application.isAccepted = isAccepted;
        application.Status = "archived";
        application.EmployeeId = empId;      
        application.DecisionDate=DateTime.Now;
        bool? updated = await _application.UpdateApplicationAsync(appId, application);
        if (updated == null)
        {
            return BadRequest("sex");
        }

        return BadRequest("GEMY");
        Notification notification;
        if (isAccepted)
        {
            notification = new Notification
            {
                Message = $"""
                           Dear {application.Student},

                           I am pleased to inform you that your recent {application.Title} request has been accepted. 
                           Please proceed with the necessary steps outlined in our guidelines.
                           
                           Thank you for your attention to this matter.
                           
                           Best regards,

                           replyed by Ana Baba yalla
                           """,
                CreatedAt = DateTime.Now,
                StudentId=application.StudentId
            };
        }
        else
        {
            notification = new Notification
            {
                Message = $"""
                           Dear {application.Student},

                           I apologize for declining your recent {application.Title} request. After careful consideration, 
                           we are unable to proceed with it at this time.
                           
                           Thank you for your understanding.

                           Best regards,

                           replyed by Ana Baba yalla
                           """,
                CreatedAt = DateTime.Now,
                StudentId=application.StudentId
            };
        }
        
        await _notification.AddNotificationAsync(notification);
        return Ok();
    }

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
            .Where(app => app.From.Replace(" ","").ToUpper() == studentName.Replace(" ","").ToUpper());
        return applicationRequestDto;
    }
    
        
    [HttpGet]
    [Route("archived/supervisor/{empId:int}/{studentName}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationArchivedDto>))]
    public async Task<IEnumerable<ApplicationArchivedDto>> GetSearchArchivedApplicationsWithMaterial(int empId,string studentName)
    {
        var Applications = await GetArchivedApplicationsWithMaterial(empId);
        var applicationArchivedDto = Applications
            .Where(app => app.From.Replace(" ","").ToUpper() == studentName.Replace(" ","").ToUpper());
        return applicationArchivedDto;
    }

#endregion

}