using AIDMS.Entities;
using AIDMS.DTOs;
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

        public async Task<bool?> AddEmployeeAsync(Employee employee)
        {
            context.Employees.Add(employee);
            int affected = await context.SaveChangesAsync();
            if (affected == 1)
            {
                return true;
            }
            return null;
        }

        public async Task<bool?> DeleteEmployeeAsync(int employeeId)
        {
            var employee = await GetEmployeeByIdAsync(employeeId);  
            if (employee == null)
            {
                return null;
            }
            context.Employees.Remove(employee);
            int affected = await context.SaveChangesAsync();
            if (affected == 1)
            {
                return true;
            }
            return null;
        }
        
        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await context.Employees.ToListAsync();
        }
        public async Task<List<Employee>> GetAllEmployeesAndRoleAsync()
        {
            return await context.Employees.Include(em => em.Role)
                .ToListAsync();
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

        public async Task<List<Employee>> GetAllSupervisorsAsync()
        {
            return await context.Employees.Where(e => e.RoleId == 3).ToListAsync();
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
        
        public async Task<bool?> UpdateEmployeeBaseInfoAsync(int employeeId, UpdateEmployeeDto employee)
        {
            var existingEmployee = await GetEmployeeByIdAsync(employeeId);
            if(existingEmployee == null) {
                throw new InvalidOperationException($"Employee with ID {employeeId} not found.");
            }

            existingEmployee.userName = employee.userName;
            existingEmployee.Password = employee.Password;
            existingEmployee.Email = employee.Email;

            context.Employees.Update(existingEmployee);
            int affected = await context.SaveChangesAsync();
            if (affected == 1)
            {
                return true;
            }
            return null;
        }
        
    }
}
