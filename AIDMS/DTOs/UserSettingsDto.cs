using System.ComponentModel.DataAnnotations;

namespace AIDMS.DTOs
{
    public class UserSettingsDto
    {
        public byte[]? profilePicture { get; set; }
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters long")]
        public string userName { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string email { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        public string password { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }
        [Required]
        public DateTime dateOfBirth { get; set; }
        [Required]
        public bool isMale { get; set; }

        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}
