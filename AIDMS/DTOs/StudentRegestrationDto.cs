using System.ComponentModel.DataAnnotations;

public class StudentRegisterationDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters long")]
    public string Username { get; set; }

    [Required]
    public string firstName { get; set; }
    [Required]
    public string lastName { get; set; }

    [Required]
    public DateTime dateOfBirth { get; set; }
    [Required]
    public bool isMale { get; set; }

    public byte[]? profilePicture { get; set; }

}
