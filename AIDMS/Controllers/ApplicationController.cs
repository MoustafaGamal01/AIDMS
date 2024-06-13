using AIDMS.DTOs;
using AIDMS.Entities;
using AIDMS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIDMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApplicationController : Controller {
    
    private readonly IApplicationRepository _application;
    public ApplicationController(IApplicationRepository application)
    {
        _application = application;
    }
    
    
    [HttpGet]
    [Route("admin")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationBaseInfoDto>))]
    public async Task<IEnumerable<ApplicationBaseInfoDto>> GetAllApplicationsBaseInfo()
    {
        var Applications = await _application.GetAllApplicationsAsync();
        var applicationBaseInfo = Applications.Select(app => new ApplicationBaseInfoDto
        {
            Id = app.Id,
            Tittle = app.Title,
            Date = app.SubmittedAt,
            DecisionDate = app.DecisionDate,
            Status = app.Status
        });
        return applicationBaseInfo;
    }
    

#region Get Application Request for the employee

    [HttpGet]
    [Route("pending/employee/{empId:int}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationRequestDto>))]
    public async Task<IEnumerable<ApplicationRequestDto>> GetPendingApplicationsExceptMaterial(int empId)
    {
        var Applications = await _application.GetAllPendingApplicationsWithStudentRelatedAsync(empId);
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
    [Route("reviewed/employee/{empId:int}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationRequestDto>))]
    public async Task<IEnumerable<ApplicationRequestDto>> GetReviewedApplicationsExceptMaterial(int empId)
    {
        var Applications = await _application.GetAllReviewedApplicationsWithStudentRelatedAsync(empId);
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
    [Route("archived/employee/{empId:int}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationArchivedDto>))]
    public async Task<IEnumerable<ApplicationArchivedDto>> GetArchivedApplicationsExceptMaterial(int empId)
    {
        var Applications = await _application.GetAllArchivedApplicationsWithStudentRelatedAsync(empId);
        var applicationArchivedDto = Applications
            .Where(application=>application.Title.ToUpper()!="Material".ToUpper())
            .Select(app => new ApplicationArchivedDto
            {
                Id = app.Id,
                Name = app.Title,
                Date = app.SubmittedAt,
                From = $"{app.Student.firstName} {app.Student.lastName}",
                Status = app.Status
            });
        return applicationArchivedDto;
    }    
#endregion


#region Get Application Request for the employee by search
    [HttpGet]
    [Route("pending/employee/{empId:int}/{studentName}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationRequestDto>))]
    [ProducesResponseType(400)]
    public async Task<IEnumerable<ApplicationRequestDto>> GetSearchInPendingApplicationsExceptMaterial(int empId,string studentName)
    {
        // return BadRequest($"{studentName}");
        var Applications = await GetPendingApplicationsExceptMaterial(empId);
        var applicationRequestDto = Applications
            .Where(app => app.From.Replace(" ","").ToUpper() == studentName.Replace(" ","").ToUpper());
        return applicationRequestDto;
    }
        
    [HttpGet]
    [Route("reviewed/employee/{empId:int}/{studentName}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationRequestDto>))]
    public async Task<IEnumerable<ApplicationRequestDto>> GetSearchInReviewedApplicationsExceptMaterial(int empId,string studentName)
    {
        var Applications = await GetReviewedApplicationsExceptMaterial(empId);
        var applicationRequestDto = Applications
            .Where(app => app.From.Replace(" ","").ToUpper() == studentName.Replace(" ","").ToUpper());
        return applicationRequestDto;
            
    }
        
    [HttpGet]
    [Route("archived/employee/{empId:int}/{studentName}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationArchivedDto>))]
    public async Task<IEnumerable<ApplicationArchivedDto>> GetSearchInArchivedApplicationsExceptMaterial(int empId,string studentName)
    {
        var Applications = await GetArchivedApplicationsExceptMaterial(empId);
        var applicationArchivedDto = Applications
            .Where(app => app.From.Replace(" ","").ToUpper() == studentName.Replace(" ","").ToUpper());
        return applicationArchivedDto;
    }
#endregion


#region Get Application Request for the supervisor
    
    [HttpGet]
    [Route("pending/supervisor/{supervisorId:int}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationRequestDto>))]
    public async Task<IEnumerable<ApplicationRequestDto>> GetPendingApplicationsWithMaterial(int supervisorId)
    {
        var Applications = await _application.GetAllPendingApplicationsWithSupervisorAsync(supervisorId);
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
    [Route("reviewed/supervisor/{supervisorId:int}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationRequestDto>))]
    public async Task<IEnumerable<ApplicationRequestDto>> GetReviewedApplicationsWithMaterial(int supervisorId)
    {
        var Applications = await _application.GetAllReviewedApplicationsWithSupervisorAsync(supervisorId);
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
    [Route("archived/supervisor/{supervisorId:int}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationArchivedDto>))]
    public async Task<IEnumerable<ApplicationArchivedDto>> GetArchivedApplicationsWithMaterial(int supervisorId)
    {
        var Applications = await _application.GetAllArchivedApplicationsWithSupervisorAsync(supervisorId);
        var applicationArchivedDto = Applications
            .Where(application=>application.Title.ToUpper()=="Material".ToUpper())
            .Select(app => new ApplicationArchivedDto
            {
                Id = app.Id,
                Name = app.Title,
                Date = app.SubmittedAt,
                From = $"{app.Student.firstName} {app.Student.lastName}",
                Status = app.Status
            });
        return applicationArchivedDto;
    }

#endregion


#region Get Application Request for the supervisor by search

    [HttpGet]
    [Route("pending/supervisor/{supervisorId:int}/{studentName}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationRequestDto>))]
    public async Task<IEnumerable<ApplicationRequestDto>> GetSearchInPendingApplicationsWithMaterial(int supervisorId,string studentName)
    {
        var Applications = await GetPendingApplicationsWithMaterial(supervisorId);
        var applicationRequestDto = Applications
            .Where(app => app.From.Replace(" ","").ToUpper() == studentName.Replace(" ","").ToUpper());
        return applicationRequestDto;
    }
        
    [HttpGet]
    [Route("reviewed/supervisor/{supervisorId:int}/{studentName}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationRequestDto>))]
    public async Task<IEnumerable<ApplicationRequestDto>> GetSearchReviewedApplicationsWithMaterial(int supervisorId,string studentName)
    {
        var Applications = await GetReviewedApplicationsWithMaterial(supervisorId);
        var applicationRequestDto = Applications
            .Where(app => app.From.Replace(" ","").ToUpper() == studentName.Replace(" ","").ToUpper());
        return applicationRequestDto;
    }
        
    [HttpGet]
    [Route("archived/supervisor/{supervisorId:int}/{studentName}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ApplicationArchivedDto>))]
    public async Task<IEnumerable<ApplicationArchivedDto>> GetSearchArchivedApplicationsWithMaterial(int supervisorId,string studentName)
    {
        var Applications = await GetArchivedApplicationsWithMaterial(supervisorId);
        var applicationArchivedDto = Applications
            .Where(app => app.From.Replace(" ","").ToUpper() == studentName.Replace(" ","").ToUpper());
        return applicationArchivedDto;
    }

#endregion
    
}