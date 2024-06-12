using System.ComponentModel.DataAnnotations;

public class CreateApplicationDto
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public IFormFile StudentDocument { get; set; }
}
