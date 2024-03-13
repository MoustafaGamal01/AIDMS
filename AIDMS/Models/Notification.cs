using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIDMS.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [StringLength(250, MinimumLength = 3, ErrorMessage = "Message must be between 2 and 50 characters")]
        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }

        public bool IsRead { get; set; }

        [Required(ErrorMessage = "Creation date is required")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public virtual Student? Student { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}
