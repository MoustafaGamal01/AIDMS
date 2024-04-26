﻿using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IRoleRepository
    {
        Task<Role> GetRoleByIdAsync(int roleId);
        Task<List<Role>> GetAllRolesAsync();
        Task AddRoleAsync(Role role);
        Task UpdateRoleAsync(int roleId, Role role);
        Task DeleteRoleAsync(int roleId);
    }
}
