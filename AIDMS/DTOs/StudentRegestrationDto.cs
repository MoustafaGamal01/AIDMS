using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    [Required(ErrorMessage = "National ID is required")]
    [StringLength(14, MinimumLength = 14, ErrorMessage = "National ID must be 14 characters long")]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be numeric and 14 digits long")]
    public string NationalID { get; set; }

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
}
