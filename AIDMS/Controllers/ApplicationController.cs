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
    
    
    // [HttpDelete("{id}")]
    // public async Task<IActionResult> Delete(int id)
    // {
    //     _application.DeleteApplicationAsync(id); 
    //     return Ok();
    // }
}