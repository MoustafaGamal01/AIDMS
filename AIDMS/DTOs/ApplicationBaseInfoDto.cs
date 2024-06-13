namespace AIDMS.DTOs;

public class ApplicationBaseInfoDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public DateTime DecisionDate { get; set; }
    public string Status { get; set; }

}