using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IApplicationRepository
    {
        Task<bool?> AddApplicationAsync(Application application);
        Task<Application> GetApplicationByIdAsync(int applicationId);
        Task<List<Application>> GetAllApplicationsAsync();
        Task<bool?> UpdateApplicationAsync(int applicationId, Application application);
        Task<bool?> DeleteApplicationAsync(int applicationId);
        Task<List<Application>> GetAllApplicationsByStudentIdAsync(int studentId);
        Task<List<Application>> GetAllApplicationsByStudentNameAsync(string studentName);
        Task<List<Application>> GetAllPendingApplicationsAsync();
        Task<List<Application>> GetAllPendingApplicationsByStudentIdAsync(int studentId);
        Task<List<Application>> GetAllPendingApplicationsByEmployeeIdAsync(int empId);
        
        Task<List<Application>> GetAllArchivedApplicationsByEmployeeIdAsync(int empId);
        
        Task<List<Application>> GetAllArchivedApplicationsAsync();
        Task<List<Application>> GetAllArchivedApplicationsByStudentIdAsync(int studentId);
    }
}
