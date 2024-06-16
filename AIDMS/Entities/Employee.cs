using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIDMS.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(1, 80, ErrorMessage = "Age must be between 1 and 80")]
        public int Age { get; set; }
        public byte[]? employeePicture { get; set; }
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "First name must be between 3 and 50 characters")]
        public string firstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Last name must be between 3 and 50 characters")]
        public string lastName { get; set; }

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
        public string phoneNumber { get; set; }

        public bool IsMale { get; set; } = true;

        [DataType(DataType.Date)]
        public DateTime dateOfBirth { get; set; }

        // Nav Prop
        [ForeignKey("Role")]

        public int? RoleId { get; set; }
        public Role Role { get; set; }
        
        public virtual List<Application>? Applications { get; set; }
        public virtual List<Notification>? Notifications { get; set; }
        
        public Employee()
        {
            Notifications = new List<Notification>();
            Applications = new List<Application>();
        }
    }
}
