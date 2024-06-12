using System.ComponentModel.DataAnnotations;
namespace AIDMS.DTOs;

public class UpdateEmployeeDto
{
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
}