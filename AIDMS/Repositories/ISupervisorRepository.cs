using AIDMS.Entities;

namespace AIDMS.Repositories;
public interface ISupervisorRepository
{
    Task<Supervisor> GetSupervisorByIdAsync(int SupervisorId);
    Task<List<Supervisor>> GetAllSupervisorAsync();
    Task AddSupervisorAsync(Supervisor Supervisor);
    Task UpdateSupervisorAsync(int SupervisorId, Supervisor Supervisor);
    Task DeleteSupervisorAsync(int SupervisorId);
}