using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IUserDetailsRepository
    {
        Task<UserDetails> GetUserDetailsByIdAsync(int userId);
<<<<<<< HEAD
        Task<UserDetails> GetUserDetailsByNameAsync(string userName);
=======
>>>>>>> main
        Task<List<UserDetails>> GetAllUsersDetailsAsync();
        Task AddUserDetailsAsync(UserDetails userDetails);
        Task UpdateUserDetailAsync(int userId, UserDetails userDetails);
        Task DeleteUserDetailAsync(int userId);
    }
}
