using System.ComponentModel.DataAnnotations;

namespace AIDMS.Entities
{
    public class Role
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "Description must not exceed 100 characters")]
        public string Description { get; set; }

        // Nav Prop
        public virtual List<Employee>? Employees { get; set; }

        public Role()
        {
            Employees = new List<Employee>();
        }

    }
}
