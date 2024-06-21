using Microsoft.AspNetCore.Identity;

namespace AIDMS.Security_Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string NationalId { get; set; }
    }
}
