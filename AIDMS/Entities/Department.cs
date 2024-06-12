namespace AIDMS.Entities
{
    public class Department
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Manager { get; set; }

        public virtual List<Student> ? Students { get; set; }
        public Department() { 
            Students = new List<Student>(); 
        }
    }
}
