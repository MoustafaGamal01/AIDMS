using Microsoft.AspNetCore.Identity;

namespace AIDMS.SecurityServices
{
    public interface ITokenService
    {
        Task<string> CreateToken(IdentityUser user);
    }
}
