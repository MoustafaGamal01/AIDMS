using System.ComponentModel.DataAnnotations.Schema;

namespace AIDMS.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        // Navigation prop
        [ForeignKey("UserDetails")]
        public int userDetailsId { get; set; }
        public virtual UserDetails? UserDetails { get; set; }

        [ForeignKey("Role")]
        public int roleId { get; set; }
        public virtual Role? Role { get; set; }

        public virtual List<Application>? Applications { get; set; }
        public virtual List<Notification>? Notifications { get; set; }
        public Employee()
        {
            Notifications = new List<Notification>();
            Applications = new List<Application>();
        }
    }
}
