using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification> GetNotificationByIdAsync(int notificationId);
        Task<Notification> GetNotificationByNameAsync(string notificationName);
        Task<List<Notification>> GetAllNotificationsAsync();
        Task AddNotificationAsync(Notification notification);
        Task UpdateNotificationAsync(int notificationId, Notification notification);
        Task DeleteNotificationAsync(int notificationId);
    }
}
