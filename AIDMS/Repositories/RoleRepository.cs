using AIDMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIDMS.Repositories;
public class RoleRepository:IRoleRepository {
    private readonly AIDMSContextClass _context;

    public RoleRepository(AIDMSContextClass context) {
        _context = context;
    }
    
    public async Task<Role> GetRoleByIdAsync(int roleId) {
        return await _context.Roles.FirstOrDefaultAsync(i => i.Id == roleId);
    }
    
    public async Task<List<Role>> GetAllRolesAsync() {
        return await _context.Roles.ToListAsync();
    }

    public async Task AddRoleAsync(Role role) {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Employee>> GetEmployeeByRoleIdAsync(int RoleId)
    {
        return await _context.Employees.Where(e => e.RoleId == RoleId).ToListAsync();
    }

    public async Task UpdateRoleAsync(int roleId, Role role) {
        var existingRole= await GetRoleByIdAsync(roleId);
        if (existingRole == null) {
            throw new InvalidOperationException($"Role with ID {roleId} not found.");
        }
        _context.Entry(existingRole).CurrentValues.SetValues(role);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRoleAsync(int roleId) {
        var roleToDelete= await GetRoleByIdAsync(roleId);
        if (roleToDelete == null) {
            throw new InvalidOperationException($"Role with ID {roleId} not found.");
        }
        _context.Roles.Remove(roleToDelete);
        await _context.SaveChangesAsync();
    }
}