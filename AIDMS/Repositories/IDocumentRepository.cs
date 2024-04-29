using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IDocumentRepository
    {
        Task<AIDocument> GetDocumentByIdAsync(int documentId);
        Task<List<AIDocument>> GetAllDocumentsByStudentIdAsync(int studentId);
        Task<List<AIDocument>> GetAllDocumentsByStudentNameAsync(string studentName);
        Task<List<AIDocument>> GetAllDocumentsAsync();
        Task AddDocumentAsync(AIDocument document);
        Task UpdateDocumentAsync(int documentId, AIDocument document);
        Task DeleteDocumentAsync(int documentId);
    }
}
