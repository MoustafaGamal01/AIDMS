using AIDMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIDMS.Repositories;

public class DepartmentRepository:IDepartmentRepository
{
    private readonly AIDMSContextClass _context;

    public DepartmentRepository(AIDMSContextClass context) {
        _context = context;
    }
    
    public async Task<Department> GetDepartmentByIdAsync(int DepartmentId)
    {
        return await _context.Departments.FirstOrDefaultAsync(i => i.Id == DepartmentId);
    }

    public async Task<List<Department>> GetAllDepartmentsAsync()
    {
        return await _context.Departments.ToListAsync();
    }

    public async Task AddDepartmentAsync(Department Department)
    {
        _context.Departments.Add(Department);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateDepartmentAsync(int DepartmentId, Department Department)
    {
        var existingDepartment= await GetDepartmentByIdAsync(DepartmentId);
        if (existingDepartment == null) {
            throw new InvalidOperationException($"Department with ID {DepartmentId} not found.");
        }
        _context.Entry(existingDepartment).CurrentValues.SetValues(Department);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDepartmentAsync(int DepartmentId)
    {
        var departmentToDelete= await GetDepartmentByIdAsync(DepartmentId);
        if (departmentToDelete == null) {
            throw new InvalidOperationException($"Department with ID {DepartmentId} not found.");
        }
        _context.Departments.Remove(departmentToDelete);
        await _context.SaveChangesAsync();
    }
}