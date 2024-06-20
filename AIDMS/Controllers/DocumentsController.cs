using System.Text;
using AIDMS.DTOs;
using AIDMS.Entities;
using AIDMS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Digests;

namespace AIDMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentsController : Controller {
    
    private readonly IApplicationRepository _application;
    private readonly IDocumentRepository _doc;
    private readonly IStudentRepository _student;

    
    public DocumentsController(IApplicationRepository application,IDocumentRepository doc,IStudentRepository student)
    {
        _application = application;
        _doc = doc;
        _student = student;
    }
    
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<DocumentDto>))]
    public async Task<IEnumerable<DocumentDto>> GetAllDocuments()
    {
        var Documents = await _doc.GetAllDocumentsAsync();
        var documents = Documents.Select(doc => new DocumentDto
        {
            FilePath = doc.FilePath,
            FileName = doc.FileName,
            FileType = doc.FileType,
            uploadedAt = doc.UploadedAt
        });
        return documents;
    }
    
    // GET: api/Documents/ByDocumentType/{documentType}/ForApplication/{applicationType}
    [HttpGet("ByDocumentType/{documentType}/ForApplication/{applicationType}")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocumentsByDocumentTypeAndApplicationType(string documentType, string applicationType)
    {
        var Documents = await _doc.GetDocumentsByDocumentTypeAndApplicationTypeAsync(documentType, applicationType);
        if (Documents == null || !Documents.Any())
        {
            return NotFound();
        }

        var documents = Documents.Select(doc => new DocumentDto
        {
            FilePath = doc.FilePath,
            FileName = doc.FileName,
            FileType = doc.FileType,
            uploadedAt = doc.UploadedAt
        });
        return Ok(documents);
    }
    
    [HttpGet("{appId:int}")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocumentsByApplicationId(int appId)
    {
        var Documents = await _doc.GetDocumentsByAppIdAndAsync(appId);
        if (Documents == null || !Documents.Any())
        {
            return NotFound();
        }
        
        var documents = Documents.Select(doc => new DocumentDto
        {
            FilePath = doc.FilePath,
            FileName = doc.FileName,
            FileType = doc.FileType,
            uploadedAt = doc.UploadedAt
        });
        return Ok(documents);
    }
    
    [HttpPost("{empId}")]
    public async Task<ActionResult<IEnumerable<string>>> MilitaryServiceDocument(int empId,[FromBody] IFormFile file)
    {
        //you should put the document here
        string document = "";
        string PID = "";
        List<string> notUpdated = new List<string>(); 
        for (int i = 0; i < document.Length; i++)
        {
            if (Char.IsDigit(document[i]))
            {
                PID += document[i];
                if (PID.Length == 14)
                {
                    bool? affected = await _student.UpdateStudentMilitaryAsync(PID);
                    if (affected == null)
                    {
                        notUpdated.Add(PID);
                    }
                    PID = "";
                }
            }
        }

        return notUpdated;
    }
    
}
