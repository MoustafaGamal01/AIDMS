using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIDMS.Entities
{
    public class Student
    {
        public int Id { get; set; }

        [Range(0.0, 4.0, ErrorMessage = "GPA must be between 0.0 and 4.0")]
        public decimal? GPA { get; set; }

        public byte[]? studentPicture { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(1, 80, ErrorMessage = "Age must be between 1 and 80")]
        public int Age { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "First name must be between 3 and 50 characters")]
        public string firstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Last name must be between 3 and 50 characters")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "required")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "SID should be 14 digit")]
        public string SID { get; set; }

        
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Username must be between 6 and 50 characters")]
        public string userName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(50, ErrorMessage = "Email must not exceed 50 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 20 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20, ErrorMessage = "Phone number must not exceed 20 characters")]
        public string PhoneNumber { get; set; }

        public bool IsMale { get; set; } = true;

        [DataType(DataType.Date)]
        public DateTime dateOfBirth { get; set; }

        public decimal? TotalPassedHours { get; set; }

        [Required(ErrorMessage = "Level is required")]
        [Range(0.0, 4.0, ErrorMessage = "Level must be between 1 and 4")]
        public int Level { get; set; }

        public bool? militaryStatus { get; set; } = false;
        // Nav Prop
        [ForeignKey("Department")]
        public int? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }

        public virtual List<Application>? Applications { get; set; }
        public virtual List<AIDocument>? Documents { get; set; }
        public virtual List<Notification>? Notifications { get; set; } 
        public Student()
        {
            Documents = new List<AIDocument>();
            Applications = new List<Application>();
            Notifications = new List<Notification>();
        }
    }
}
