using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IApplicationRepository
    {
        Task<Application> GetApplicationByIdAsync(int applicationId);
        Task<List<Application>> GetAllApplicationsAsync();
        Task AddApplicationAsync(Application application);
        Task UpdateApplicationAsync(int applicationId, Application application);
        Task DeleteApplicationAsync(int applicationId);
        Task<List<Application>> GetAllApplicationsByStudentIdAsync(int studentId);
        Task<List<Application>> GetAllApplicationsByStudentNameAsync(string studentName);
        Task<List<Application>> GetAllReviewedApplicationsAsync();
        Task<List<Application>> GetAllReviewedApplicationsByStudentIdAsync(int studentId);
        Task<List<Application>> GetAllArchivedApplicationsAsync();
        Task<List<Application>> GetAllArchivedApplicationsByStudentIdAsync(int studentId);
        Task<List<Application>> GetAllPendingApplicationsAsync();
        Task<List<Application>> GetAllPendingApplicationsByStudentIdAsync(int studentId);
        Task<List<Application>> GetAllPendingApplicationsWithStudentRelatedAsync(int empId);
        Task<List<Application>> GetAllReviewedApplicationsWithStudentRelatedAsync(int empId);
        Task<List<Application>> GetAllArchivedApplicationsWithStudentRelatedAsync(int empId);
        Task<List<Application>> GetAllArchivedApplicationsWithSupervisorAsync(int supervisorId);
        Task<List<Application>> GetAllPendingApplicationsWithSupervisorAsync(int supervisorId);
        Task<List<Application>> GetAllReviewedApplicationsWithSupervisorAsync(int supervisorId);
    }
}
