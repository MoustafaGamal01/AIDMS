using System.ComponentModel.DataAnnotations.Schema;

namespace AIDMS.Entities
{
    public class Student
    {
        public int Id { get; set; }

        // Navigation prop
        [ForeignKey("UserDetails")]
        public int userDetailsId { get; set; }
        public virtual UserDetails? UserDetails { get; set; }

        public virtual List<Application>? Applications { get; set; }
        public virtual List<AIDocument>? Documents { get; set; }
        public virtual List<Notification>? Notifications { get; set; }

        public Student()
        {
            Documents = new List<AIDocument>();
            Applications = new List<Application>();
            Notifications = new List<Notification>();
        }
    }
}
