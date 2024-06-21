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
        Task UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto employee);
        Task<bool?> DeleteEmployeeAsync(int employeeId);
        //Task<bool?> UpdateEmployeeBaseInfoAsync(int employeeId, UpdateEmployeeDto employee);
    }
}
