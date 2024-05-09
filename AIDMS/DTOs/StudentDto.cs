using System.Reflection.Metadata;
using AIDMS.Entities;
namespace AIDMS.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public decimal? GPA { get; set; }
        public string studentDepartment { get; set; }
        public decimal? TotalPassedHours { get; set; }
        public int Level { get; set; }
        public List<AIDocument>? studentDocuments { get; set; }
    }
}
