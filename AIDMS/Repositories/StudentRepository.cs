using AIDMS.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIDMS.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AIDMSContextClass _context;

        public StudentRepository(AIDMSContextClass context)
        {
            _context = context;
        }

        public async Task<Student> GetStudentByIdAsync(int studentId)
        {
            return await _context.Students.FirstOrDefaultAsync(i => i.Id == studentId);
        }

        public async Task<Student> GetStudentByNameAsync(string studentName)
        {
            return await _context.Students
                .Include(s => s.UserDetails)
                .FirstOrDefaultAsync(s => s.UserDetails.FirstName == studentName || s.UserDetails.LastName == studentName);
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task AddStudentAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStudentAsync(int studentId, Student student)
        {
            var existingStudent = await GetStudentByIdAsync(studentId);
            if (existingStudent == null)
            {
                throw new InvalidOperationException($"Student with ID {studentId} not found.");
            }
            
            _context.Entry(existingStudent).CurrentValues.SetValues(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudentAsync(int studentId)
        {
            var studentToDelete = await GetStudentByIdAsync(studentId);
            if (studentToDelete == null)
            {
                throw new InvalidOperationException($"Student with ID {studentId} not found.");
            }

            _context.Students.Remove(studentToDelete);
            await _context.SaveChangesAsync();
        }
    }
}
