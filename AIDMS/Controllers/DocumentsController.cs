using AIDMS.DTOs;
using AIDMS.Entities;
using AIDMS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIDMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentsController : Controller {
    
    private readonly IApplicationRepository _application;
    private readonly IDocumentRepository _doc;
    
    public DocumentsController(IApplicationRepository application,IDocumentRepository doc)
    {
        _application = application;
        _doc = doc;
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
            FilePath = doc.FilePath
        });
        return Ok(documents); 
            
    }
    
    // [HttpGet]
    // //[ProducesResponseType(200, Type = typeof(IEnumerable<DocumentDto>))]
    // public async Task<IEnumerable<DocumentDto>> GetAllDocuments()
    // {
    //     var Documents = await _doc.GetAllDocumentsAsync();
    //     var documents = Documents.Select(doc => new DocumentDto
    //     {
    //         FilePath = doc.FilePath
    //     });
    //     return documents;
    // }
    
    
    
}
