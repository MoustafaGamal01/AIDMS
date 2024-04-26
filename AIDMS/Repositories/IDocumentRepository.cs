using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IDocumentRepository
    {
        Task<AIDocument> GetDocumentByIdAsync(int documentId);
        Task<List<AIDocument>> GetAllDocumentsAsync();
        Task AddDocumentAsync(AIDocument document);
        Task UpdateDocumentAsync(int documentId, AIDocument document);
        Task DeleteDocumentAsync(int documentId);
    }
}
