using AIDMS.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace AIDMS.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AIDMSContextClass context;
        private readonly IRoleRepository _role;

        public EmployeeRepository(AIDMSContextClass context, IRoleRepository role) {
            this.context = context;
            this._role = role;
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            context.Employees.Add(employee);
            await context.SaveChangesAsync();
        }

       

        public async Task DeleteEmployeeAsync(int employeeId)
        {
            var employee = await GetEmployeeByIdAsync(employeeId);  
            if (employee == null)
            {
                throw new InvalidOperationException($"Employee with ID {employeeId} not found.");
            }
            context.Employees.Remove(employee); 
            await context.SaveChangesAsync();
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await context.Employees.ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            return await context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        }

        public async Task<Employee> GetEmployeeByNameAsync(string employeeName)
        {
            return await context.Employees
                .FirstOrDefaultAsync(e => e.firstName.Contains(employeeName) || e.lastName.Contains("employeeName"));
        }

        public async Task UpdateEmployeeAsync(int employeeId, Employee employee)
        {
            var existingEmployee = await GetEmployeeByIdAsync(employeeId);
            if(existingEmployee == null) {
                throw new InvalidOperationException($"Employee with ID {employeeId} not found.");
            }
            context.Entry(existingEmployee).CurrentValues.SetValues(employee);
            await context.SaveChangesAsync();
        }
    }
}
