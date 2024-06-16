using AIDMS.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AIDMS.Repositories;
public class NotificationRepository:INotificationRepository {
    private readonly AIDMSContextClass _context;

    public NotificationRepository(AIDMSContextClass context) {
        _context = context;
    }
    public async Task<Notification> GetNotificationByIdAsync(int notificationId) {
        return await _context.Notifications.FirstOrDefaultAsync(i => i.Id == notificationId);
    }
    public async Task<List<Notification>> GetAllNotificationsAsync() {
        return await _context.Notifications.ToListAsync();
    }

    public async Task<bool?> AddNotificationAsync(Notification notification) {
        _context.Notifications.Add(notification);
        
        int affected = await _context.SaveChangesAsync();
        if (affected == 1)
        {
            return true;
        }
        return null;
    }

    public async Task UpdateNotificationAsync(int notificationId, Notification notification) {
        var existingNotification= await GetNotificationByIdAsync(notificationId);
        if (existingNotification == null) {
            throw new InvalidOperationException($"Notification with ID {notificationId} not found.");
        }
        _context.Entry(existingNotification).CurrentValues.SetValues(notification);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteNotificationAsync(int notificationId) {
        var notificationToDelete= await GetNotificationByIdAsync(notificationId);
        if (notificationToDelete == null) {
            throw new InvalidOperationException($"Notification with ID {notificationId} not found.");
        }

        _context.Notifications.Remove(notificationToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Notification>> GetAllNotificationsByStudentIdAsync(int studentId)
    {
        return await _context.Notifications.Where(i => i.StudentId == studentId && i.fromStudent == false).ToListAsync();
    }

    public async Task<List<Notification>> GetAllNotificationsByEmployeeIdAsync(int empId)
    {
        return await _context.Notifications.Where(i => i.EmployeeId == empId).ToListAsync();
    }
}