using System.ComponentModel.DataAnnotations;

namespace AIDMS.DTOs
{
    public class EmployeeRegestrationDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters long")]
        public string Username { get; set; }

        public byte[]? employeePicture { get; set; }

        [Required]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be numeric and 14 digits long")]
        public string nationalId { get; set; }

        [Required]
        public bool isMale { get; set; }

        [Required]
        public DateTime dateOfBirth { get; set; }

        [Required]
        public string role { get; set; }
        // Temporarily Added
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}
