using AIDMS.DTOs;
using AIDMS.Entities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using AIDMS.Security_Entities;

namespace AIDMS.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AIDMSContextClass _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentRepository(AIDMSContextClass context, UserManager<ApplicationUser>userManager)
        {
            _context = context;
            this._userManager = userManager;
        }

        public async Task<Student> GetAllStudentDataByIdAsync(int studentId)
        {
            return await _context.Students.FirstOrDefaultAsync(i => i.Id == studentId);
        }

        public async Task<Student> GetAllStudentDataByNameAsync(string studentName)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.firstName.Contains(studentName)  || s.lastName.Contains(studentName));
        }

        public async Task<Student> GetStudentPersonalInfoByIdAsync(int studentId)
        {
            return await _context.Students.Include("Department").FirstOrDefaultAsync(i => i.Id == studentId);
        }

        public async Task<Student> GetStudentPersonalInfoByNameAsync(string studentName)
        {
            return await _context.Students.Include("Department").FirstOrDefaultAsync(i => (i.firstName+' '+i.lastName) == studentName);
        }

        public async Task<List<Student>> GetAllStudentsDataAsync()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<List<Student>> GetAllStudentsPersonalInfoAsync()
        {
            return await _context.Students.Include("Department").ToListAsync();
        }

        public async Task<bool?> AddStudentAsync(Student student)
        {
            _context.Students.Add(student);
            int affected = await _context.SaveChangesAsync();
            return (affected >= 1);
        }


        public async Task UpdateStudentAsync(int studentId, UserSettingsDto userSettingsDto)
        {
            var existingStudent = await _context.Students.FindAsync(studentId);
            if (existingStudent == null)
            {
                throw new KeyNotFoundException("Student not found");
            }

            var user = await _userManager.FindByNameAsync(existingStudent.userName);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with username: {existingStudent.userName} not found.");
            }

            // Update profile picture if provided
            if (userSettingsDto.profilePicture != null)
            {
                existingStudent.studentPicture = userSettingsDto.profilePicture;
            }

            // Update username if provided
            if (!string.IsNullOrEmpty(userSettingsDto.userName))
            {
                if (!IsValidUsername(userSettingsDto.userName))
                {
                    throw new ArgumentException("Invalid username. It must be at least 4 characters long, alphanumeric, and can contain periods and underscores.");
                }
                existingStudent.userName = userSettingsDto.userName;
                user.UserName = userSettingsDto.userName;
            }

            // Update email if provided
            if (!string.IsNullOrEmpty(userSettingsDto.email))
            {
                if (!IsValidEmail(userSettingsDto.email))
                {
                    throw new ArgumentException("Invalid email format.");
                }
                user.Email = userSettingsDto.email;
            }

            // Update phone number if provided
            if (!string.IsNullOrEmpty(userSettingsDto.Phone))
            {
                if (!IsValidEgyptianPhoneNumber(userSettingsDto.Phone))
                {
                    throw new ArgumentException("Invalid phone number. It must be a valid Egyptian phone number.");
                }
                existingStudent.PhoneNumber = userSettingsDto.Phone;
                user.PhoneNumber = userSettingsDto.Phone;
            }

            if (!string.IsNullOrEmpty(userSettingsDto.CurrentPassword) &&
                !string.IsNullOrEmpty(userSettingsDto.NewPassword) &&
                !string.IsNullOrEmpty(userSettingsDto.ConfirmPassword))
            {
                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, userSettingsDto.CurrentPassword, userSettingsDto.NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    var errors = string.Join(", ", passwordChangeResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to change password: {errors}");
                }
            }


            _context.Students.Update(existingStudent);
            await _context.SaveChangesAsync();
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
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

        public async Task<bool?> DeleteStudentAsync(int studentId)
        {
            var studentToDelete = await GetAllStudentDataByIdAsync(studentId);
            if (studentToDelete == null)
            {
                throw new InvalidOperationException($"Student with ID {studentId} not found.");
            }

            _context.Students.Remove(studentToDelete);
            int affected = await _context.SaveChangesAsync();
            if (affected == 1)
            {
                return true;
            }

            return null;
        }

        public async Task<Student> GetStudentByNationalIdAsync(string NatId)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.SID == NatId);
        }

        public async Task<bool?> UpdateStudentMilitaryAsync(string PID)
        {
            var student = await _context.Students.FirstOrDefaultAsync(stu => stu.SID == PID);
            if (student == null)
            {
                return null;
            }

            student.militaryEducation = true;
            _context.Update(student);
            int affected = await _context.SaveChangesAsync();
            if (affected == 1)
            {
                return true;
            }

            return null;
        }
    }
}
