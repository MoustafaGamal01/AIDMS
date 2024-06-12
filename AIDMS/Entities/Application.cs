using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIDMS.Entities
{
    public class Application
    {
        public int Id { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 50 characters")]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Satus must be between 2 and 50 characters")]
        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }

        public bool isArchived { get; set; } = false; 

        [StringLength(150, MinimumLength = 2, ErrorMessage = "Description must be between 2 and 150 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Submitted date is required")]
        [DataType(DataType.DateTime)]
        public DateTime SubmittedAt { get; set; }

        [Required(ErrorMessage = "Decision date is required")]
        [DataType(DataType.DateTime)]
        public DateTime DecisionDate { get; set; }

        [Required(ErrorMessage = "Review date is required")]
        [DataType(DataType.DateTime)]
        public DateTime ReviewDate { get; set; }

        // Navigation properties
        [ForeignKey("Student")]
        public int? StudentId { get; set; }
        public virtual Student? Student { get; set; }

        [ForeignKey("Employee")]
        public int? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }

        [ForeignKey("Payment")] 
        public int? PaymentId { get; set; }
        public virtual Payment? Payment { get; set; }

        [ForeignKey("Supervisor")]
        public int? SupervisorId { get; set; }
        public virtual Supervisor? Supervisor { get; set; }

        public virtual List<AIDocument>? Documents { get; set; }
        public Application()
        {
            Documents = new List<AIDocument>();
        }
    }
}
