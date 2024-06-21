using System.ComponentModel.DataAnnotations;

public class UpdateEmployeeDto
{
    [StringLength(50, MinimumLength = 6, ErrorMessage = "Username must be between 6 and 50 characters")]
    public string? UserName { get; set; }

    public string? Email { get; set; }

    [DataType(DataType.Password)]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 20 characters")]
    public string? CurrentPassword { get; set; }

    [DataType(DataType.Password)]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 20 characters")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string? ConfirmPassword { get; set; }

    public byte[]? EmpProfilePicture { get; set; }

    public string? PhoneNumber { get; set; }
}
