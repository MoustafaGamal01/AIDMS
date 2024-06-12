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
        // New methods
        // Task<List<AIDocument>> GetDocumentsByApplicationTypeAsync(string applicationType);
        Task<List<AIDocument>> GetDocumentsByDocumentTypeAndApplicationTypeAsync(string documentType, string applicationType);
    }
}
