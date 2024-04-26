using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIDMS.Entities
{
    public class AIDocument
    {
        public int Id { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "File Name must be between 2 and 50 characters")]
        [Required(ErrorMessage = "File name is required")]
        public string FileName { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "File Type must be between 2 and 50 characters")]
        [Required(ErrorMessage = "File type is required")]
        public string FileType { get; set; }

        [StringLength(255, MinimumLength = 2, ErrorMessage = "File Path must be between 2 and 255 characters")]
        [Required(ErrorMessage = "File path is required")]
        public string FilePath { get; set; }

        [Required(ErrorMessage = "Uploaded date is required")]
        [DataType(DataType.DateTime)]
        public DateTime UploadedAt { get; set; }

        // Navigation properties
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public virtual Student? Student { get; set; }

        [ForeignKey("Application")]
        public int ApplicationId { get; set; }
        public virtual Application? Application { get; set; }
    }
}
