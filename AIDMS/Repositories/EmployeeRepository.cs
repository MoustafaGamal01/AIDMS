using AIDMS.Entities;
using AIDMS.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Net.Mime.MediaTypeNames;
using Google.Api;
using Microsoft.AspNetCore.Identity;
using AIDMS.Security_Entities;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

namespace AIDMS.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AIDMSContextClass context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeRepository(AIDMSContextClass context, UserManager<ApplicationUser> userManager) {
            this.context = context;
            this._userManager = userManager;
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

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            
            return await context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        }

        public async Task<Employee> GetEmployeeByNameAsync(string employeeName)
        {
            return await context.Employees
                .FirstOrDefaultAsync(e => e.firstName.Contains(employeeName) || e.lastName.Contains("employeeName"));
        }

        public async Task UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto employeeDto)
        {
            var existingEmp = await context.Employees.FindAsync(employeeId);
            if (existingEmp == null)
            {
                throw new KeyNotFoundException($"Employee with ID: {employeeId} not found.");
            }

            var user = await _userManager.FindByNameAsync(existingEmp.userName);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with username: {existingEmp.userName} not found.");
            }

            // Update profile picture if provided
            if (employeeDto.EmpProfilePicture != null)
            {
                existingEmp.employeePicture = employeeDto.EmpProfilePicture;
            }

            // Update username if provided
            if (!string.IsNullOrEmpty(employeeDto.UserName))
            {
                if (!IsValidUsername(employeeDto.UserName))
                {
                    throw new ArgumentException("Invalid username. It must be at least 4 characters long, alphanumeric, and can contain periods and underscores.");
                }
                existingEmp.userName = employeeDto.UserName;
                user.UserName = employeeDto.UserName;
            }

            // Update email if provided
            if (!string.IsNullOrEmpty(employeeDto.Email))
            {
                if (!IsValidEmail(employeeDto.Email))
                {
                    throw new ArgumentException("Invalid email format.");
                }
                user.Email = employeeDto.Email;
            }

            // Update phone number if provided
            if (!string.IsNullOrEmpty(employeeDto.PhoneNumber))
            {
                if (!IsValidEgyptianPhoneNumber(employeeDto.PhoneNumber))
                {
                    throw new ArgumentException("Invalid phone number. It must be a valid Egyptian phone number.");
                }
                existingEmp.phoneNumber = employeeDto.PhoneNumber;
                user.PhoneNumber = employeeDto.PhoneNumber;
            }

            // Update password if provided
            if (!string.IsNullOrEmpty(employeeDto.CurrentPassword) &&
                !string.IsNullOrEmpty(employeeDto.NewPassword) &&
                !string.IsNullOrEmpty(employeeDto.ConfirmPassword))
            {
                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, employeeDto.CurrentPassword, employeeDto.NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    var errors = string.Join(", ", passwordChangeResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to change password: {errors}");
                }
            }

            context.Employees.Update(existingEmp);
            await context.SaveChangesAsync();

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed to update user: {errors}");
            }
        }

        #region Validations

        private bool IsValidEgyptianPhoneNumber(string phoneNumber)
        {
            // Regex pattern to match Egyptian mobile and landline phone numbers
            var regex = new Regex(@"^(01[0125][0-9]{8}|0[2-5][0-9]{7,8})$");
            return regex.IsMatch(phoneNumber);
        }
        // Method to hash password
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        // Method to validate password complexity
        private bool IsValidPassword(string password)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");
            var hasSpecialChar = new Regex(@"[!@#$%^&*(),.?""{}|<>]");

            return hasNumber.IsMatch(password) &&
                   hasUpperChar.IsMatch(password) &&
                   hasLowerChar.IsMatch(password) &&
                   hasMinimum8Chars.IsMatch(password) &&
                   hasSpecialChar.IsMatch(password);
        }

        // Method to validate username similar to Instagram
        private bool IsValidUsername(string username)
        {
            var regex = new Regex(@"^[a-zA-Z0-9._]{4,}$");
            return regex.IsMatch(username);
        }

        // Method to validate email format
        private bool IsValidEmail(string email)
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }

        #endregion

        //public async Task<bool?> UpdateEmployeeBaseInfoAsync(int employeeId, UpdateEmployeeDto employee)
        //{
        //    var existingEmployee = await GetEmployeeByIdAsync(employeeId);
        //    if (existingEmployee == null)
        //    {
        //        return null;
        //        throw new InvalidOperationException($"Employee with ID {employeeId} not found.");
        //    }

        //    existingEmployee.userName = employee.userName;
        //    context.Employees.Update(existingEmployee);
        //    int affected = await context.SaveChangesAsync();
        //    if (affected == 1)
        //    {
        //        return true;
        //    }
        //    return null;
        //}

    }
}
