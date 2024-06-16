using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification> GetNotificationByIdAsync(int notificationId);
        Task<List<Notification>> GetAllNotificationsByStudentIdAsync(int studentId);
        Task<List<Notification>> GetAllNotificationsByEmployeeIdAsync(int empId);
        Task<bool?> AddNotificationAsync(Notification notification);
        Task UpdateNotificationAsync(int notificationId, Notification notification);
        Task DeleteNotificationAsync(int notificationId);
    }
}
