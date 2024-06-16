using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IApplicationRepository
    {
        Task<Application> GetApplicationByIdAsync(int applicationId);
        Task<List<Application>> GetAllApplicationsAsync();
        Task<bool?> UpdateApplicationAsync(int applicationId, Application application);
        Task DeleteApplicationAsync(int applicationId);
        Task<List<Application>> GetAllApplicationsByStudentIdAsync(int studentId);
        Task<List<Application>> GetAllApplicationsByStudentNameAsync(string studentName);
        Task<List<Application>> GetAllPendingApplicationsAsync();
        Task<List<Application>> GetAllPendingApplicationsByStudentIdAsync(int studentId);
        Task<List<Application>> GetAllPendingApplicationsByEmployeeIdAsync(int empId);
        
<<<<<<< HEAD
        // Task<List<Application>> GetAllArchivedApplicationsAsync();
        // Task<List<Application>> GetAllArchivedApplicationsByStudentIdAsync();
        //Task<List<Application>> GetAllReviewedApplicationsWithStudentRelatedAsync(int empId);
        Task<List<Application>> GetAllArchivedApplicationsWithStudentRelatedAsync();
=======
>>>>>>> main
        Task<List<Application>> GetAllArchivedApplicationsByEmployeeIdAsync(int empId);
        
        Task<List<Application>> GetAllArchivedApplicationsAsync();
        Task<List<Application>> GetAllArchivedApplicationsByStudentIdAsync(int studentId);
    }
}
