using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AIDMS.DTOs
{
    public class UploadDocumentDto
    {
        [Required]
        public int step { get; set; }
        [Required]
        public int studentId { get; set; }
        [Required]
        public IFormFile file { get; set; }
    }
}
