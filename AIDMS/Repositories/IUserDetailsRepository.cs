using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IUserDetailsRepository
    {
        Task<UserDetails> GetUserDetailsByIdAsync(int userId);
        Task<UserDetails> GetUserDetailsByNameAsync(string userName);
        Task<List<UserDetails>> GetAllUsersDetailsAsync();
        Task AddUserDetailsAsync(UserDetails userDetails);
        Task UpdateUserDetailAsync(int userId, UserDetails userDetails);
        Task DeleteUserDetailAsync(int userId);
    }
}
