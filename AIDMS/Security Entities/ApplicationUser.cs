using Microsoft.AspNetCore.Identity;

namespace AIDMS.Security_Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string NationalId { get; set; }
        public string UserType { get; set; }
        public int? EmpId { get; set; }
        public int? StdId { get; set; }
    }
}
