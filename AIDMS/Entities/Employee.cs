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

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20, ErrorMessage = "Phone number must not exceed 20 characters")]
        public string phoneNumber { get; set; }

        public bool IsMale { get; set; } = true;

        [DataType(DataType.Date)]
        public DateTime dateOfBirth { get; set; }
        
        public virtual List<Application>? Applications { get; set; }
        public virtual List<Notification>? Notifications { get; set; }
        
        public Employee()
        {
            Notifications = new List<Notification>();
            Applications = new List<Application>();
        }
    }
}
