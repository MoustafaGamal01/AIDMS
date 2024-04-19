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
            return await _context.Applications.FirstOrDefaultAsync(app =>app.Id == applicationId);
        }

        public async Task<List<Application>> GetAllApplicationsAsync()
        {
            return await _context.Applications.ToListAsync();
        }

        public async Task<List<Application>> GetApplicationsForCertainStudnetAsync(int studentId)
        {
            return await _context.Applications
                .Where(a => a.StudentId == studentId)
                .ToListAsync();
        }

        public async Task AddApplicationAsync(Application application)
        {
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateApplicationAsync(int applicationId, Application application)
        {
            var existingApplication = await GetApplicationByIdAsync(applicationId);
            if (existingApplication == null)
            {
                throw new InvalidOperationException($"Application with ID {applicationId} not found.");
            }

            _context.Entry(existingApplication).CurrentValues.SetValues(application);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteApplicationAsync(int applicationId)
        {
            var applicationToDelete = await GetApplicationByIdAsync(applicationId);
            if (applicationToDelete == null)
            {
                throw new InvalidOperationException($"Application with ID {applicationId} not found.");
            }

            _context.Applications.Remove(applicationToDelete);
            await _context.SaveChangesAsync();
        }
    }
}
