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
        public virtual List<Document>? Documents { get; set; }
        public virtual List<Notification>? Notifications { get; set; }

        public Student()
        {
            Documents = new List<Document>();
            Applications = new List<Application>();
            Notifications = new List<Notification>();
        }
    }
}
