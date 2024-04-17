using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployeeByIdAsync(int employeeId);
        Task<Employee> GetEmployeeByNameAsync(string employeeName);
        Task<List<Employee>> GetAllEmployeesAsync();
        Task AddEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(int employeeId, Employee employee);
        Task DeleteEmployeeAsync(int employeeId);
    }
}
