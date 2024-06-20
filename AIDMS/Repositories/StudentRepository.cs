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

namespace AIDMS.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AIDMSContextClass _context;

        public StudentRepository(AIDMSContextClass context)
        {
            _context = context;
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
            }

            // Update email if provided
            if (!string.IsNullOrEmpty(userSettingsDto.email))
            {
                if (!IsValidEmail(userSettingsDto.email))
                {
                    throw new ArgumentException("Invalid email format.");
                }
                existingStudent.Email = userSettingsDto.email;
            }

            // Update password if provided (hashed)
            if (!string.IsNullOrEmpty(userSettingsDto.password))
            {
                if (!IsValidPassword(userSettingsDto.password))
                {
                    throw new ArgumentException("Invalid password. It must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one special character, and one number.");
                }
                existingStudent.Password = userSettingsDto.password;
            }

            // Update phone number if provided
            if (!string.IsNullOrEmpty(userSettingsDto.Phone))
            {
                if (!IsValidEgyptianPhoneNumber(userSettingsDto.Phone))
                {
                    throw new ArgumentException("Invalid phone number. It must be a valid Egyptian phone number.");
                }
                existingStudent.PhoneNumber = userSettingsDto.Phone;
            }

            _context.Students.Update(existingStudent);
            await _context.SaveChangesAsync();
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

        public async Task<bool?> UpdateStudentMilitaryAsync(string PID)
        {
            var student = await _context.Students.FirstOrDefaultAsync(stu => stu.SID == PID);
            student.militaryStatus = true;
            _context.Students.Update(student);
            int affected = await _context.SaveChangesAsync();
            if (affected == 1)
            {
                return true;
            }

            return null;
        }

    }
}
