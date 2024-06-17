using AIDMS.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIDMS.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly AIDMSContextClass _context;

        public ApplicationRepository(AIDMSContextClass context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Application> GetApplicationByIdAsync(int applicationId)
        {
            return await _context.Applications.FirstOrDefaultAsync(app => app.Id == applicationId);
        }

        public async Task<List<Application>> GetAllApplicationsAsync()
        {
            return await _context.Applications.ToListAsync();
        }
        
        public async Task<List<Application>> GetAllApplicationsByStudentIdAsync(int studentId)
        {
            return await _context.Applications.Where(i => i.StudentId == studentId).ToListAsync();
        }

        public async Task<List<Application>> GetAllApplicationsByStudentNameAsync(string studentName)
        {
            var std = await _context.Students
                .SingleOrDefaultAsync(s => ((s.firstName.Contains(studentName)) ||
                                            (s.lastName.Contains(studentName)) ||
                                            (s.firstName + " " + s.lastName).Contains(studentName)));

            if (std == null)
            {
                return new List<Application>();
            }

            var allDocuments = await _context.Applications
                .Where(a => a.StudentId == std.Id)
                .ToListAsync();

            return allDocuments;
        }

        public async Task<List<Application>> GetAllArchivedApplicationsByStudentIdAsync(int studentId)
        {
            return await _context.Applications.Where(i => i.StudentId == studentId && i.Status != "Pending")
                .ToListAsync();
        }
        

        public async Task<List<Application>> GetAllPendingApplicationsByStudentIdAsync(int studentId)
        {
            return await _context.Applications.Where(i => i.StudentId == studentId && i.Status == "Pending")
                .ToListAsync();
        }

        public async Task<List<Application>> GetAllPendingApplicationsByEmployeeIdAsync(int empId)
        {
            return await _context.Applications.Include(app => app.Student)
                .Where(i => i.EmployeeId == empId && i.Status == "Pending").ToListAsync();
        }
        

        public async Task<List<Application>> GetAllArchivedApplicationsByEmployeeIdAsync(int empId)
        {
            return await _context.Applications.Include(app => app.Student)
                .Where(i => i.EmployeeId == empId && i.Status.ToUpper() == "Archived")
                .ToListAsync();
        }

        public async Task<bool?> UpdateApplicationAsync(int applicationId, Application application)
        {
            var existingApplication = await GetApplicationByIdAsync(applicationId);
            if (existingApplication == null)
            {
                return null;
                // throw new InvalidOperationException($"Application with ID {applicationId} not found.");
            }

            _context.Entry(existingApplication).CurrentValues.SetValues(application);
            int affected = await _context.SaveChangesAsync();
            if (affected == 1)
            {
                return true;
            }

            return null;
        }

        public async Task<bool?> DeleteApplicationAsync(int applicationId)
        {
            var applicationToDelete = await GetApplicationByIdAsync(applicationId);
            if (applicationToDelete == null)
            {
                return null;
            }

            _context.Applications.Remove(applicationToDelete);
            int affected = await _context.SaveChangesAsync();
            if (affected == 1)
            {
                return true;
            }

            return null;
        }

        public async Task<List<Application>> GetAllArchivedApplicationsAsync()
        {
            return await _context.Applications.Where(i => i.Status != "Pending")
                .Include(app => app.Student)
                .ToListAsync();
        }


        public async Task<List<Application>> GetAllPendingApplicationsAsync()
        {
            return await _context.Applications
                .Where(i => i.Status.ToUpper() == "Pending".ToUpper())
                .Include(app => app.Student)
                .ToListAsync();
        }

    }
}
