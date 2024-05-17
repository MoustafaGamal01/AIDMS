using AIDMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIDMS.Repositories;
public class SupervisorRepository:ISupervisorRepository
{
    private readonly AIDMSContextClass _context;

    public SupervisorRepository(AIDMSContextClass context) {
        _context = context;
    }
    
    public async Task<Supervisor> GetSupervisorByIdAsync(int SupervisorId)
    {
        return await _context.Supervisors.FirstOrDefaultAsync(i => i.Id == SupervisorId);
    }

    public async Task<List<Supervisor>> GetAllSupervisorAsync()
    {
        return await _context.Supervisors.ToListAsync();
    }

    public async Task AddSupervisorAsync(Supervisor Supervisor)
    {
        _context.Supervisors.Add(Supervisor);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSupervisorAsync(int SupervisorId, Supervisor Supervisor)
    {
        var existingSupervisor= await GetSupervisorByIdAsync(SupervisorId);
        if (existingSupervisor == null) {
            throw new InvalidOperationException($"Supervisor with ID {SupervisorId} not found.");
        }
        _context.Entry(existingSupervisor).CurrentValues.SetValues(Supervisor);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSupervisorAsync(int SupervisorId)
    {
        var SupervisorToDelete= await GetSupervisorByIdAsync(SupervisorId);
        if (SupervisorToDelete == null) {
            throw new InvalidOperationException($"Supervisor with ID {SupervisorId} not found.");
        }

        _context.Supervisors.Remove(SupervisorToDelete);
        await _context.SaveChangesAsync();
    }
}