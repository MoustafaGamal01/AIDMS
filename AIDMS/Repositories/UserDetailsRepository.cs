using AIDMS.Entities;
using Microsoft.EntityFrameworkCore;


namespace AIDMS.Repositories;
public class UserDetailsRepository:IUserDetailsRepository {
    private readonly AIDMSContextClass _context;

    public UserDetailsRepository(AIDMSContextClass context) {
        _context = context;
    }
    
    public async Task<UserDetails> GetUserDetailsByIdAsync(int userId) {
        return await _context.UserDetails.FirstOrDefaultAsync(i => i.Id == userId);
    }
    public async Task<List<UserDetails>> GetAllUsersDetailsAsync() {
        return await _context.UserDetails.ToListAsync();
    }

    public async Task AddUserDetailsAsync(UserDetails userDetails) {
        _context.UserDetails.Add(userDetails);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserDetailAsync(int userId, UserDetails userDetails) {
        var existingUser = await GetUserDetailsByIdAsync(userId);
        if (existingUser == null) {
            throw new InvalidOperationException($"UserDetails with ID {userId} not found.");
        }
        _context.Entry(existingUser).CurrentValues.SetValues(userDetails);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserDetailAsync(int userId) {
        var userToDelete = await GetUserDetailsByIdAsync(userId);
        if (userToDelete == null) {
            throw new InvalidOperationException($"Notification with ID {userId} not found.");
        }

        _context.UserDetails.Remove(userToDelete);
        await _context.SaveChangesAsync();
    }
}