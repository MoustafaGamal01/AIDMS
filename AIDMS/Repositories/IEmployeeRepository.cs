using AIDMS.Entities;
using AIDMS.DTOs;
namespace AIDMS.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployeeByIdAsync(int employeeId);
        Task<Employee> GetEmployeeByNameAsync(string employeeName);
        Task<List<Employee>> GetAllEmployeesAsync();
        Task<bool?> AddEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(int employeeId, Employee employee);
        Task<bool?> DeleteEmployeeAsync(int employeeId);
        Task<List<Employee>> GetAllEmployeesAndRoleAsync();
        Task<bool?> UpdateEmployeeBaseInfoAsync(int employeeId, UpdateEmployeeDto employee);
    }
}
