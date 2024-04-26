using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IRoleRepository
    {
        Task<Role> GetRoleByIdAsync(int roleId);
<<<<<<< HEAD
<<<<<<< HEAD
        Task<Role> GetRoleByNameAsync(string roleName);
=======
>>>>>>> main
=======
>>>>>>> main
        Task<List<Role>> GetAllRolesAsync();
        Task AddRoleAsync(Role role);
        Task UpdateRoleAsync(int roleId, Role role);
        Task DeleteRoleAsync(int roleId);
    }
}
