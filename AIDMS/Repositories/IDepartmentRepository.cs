using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IDepartmentRepository
    {
        Task<Department> GetDepartmentByIdAsync(int DepartmentId);
        Task<List<Department>> GetAllDepartmentsAsync();
        Task AddDepartmentAsync(Department Department);
        Task UpdateDepartmentAsync(int DepartmentId, Department Department);
        Task DeleteDepartmentAsync(int DepartmentId);
    }
}
