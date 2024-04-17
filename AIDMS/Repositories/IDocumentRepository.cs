using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IDocumentRepository
    {
        Task<Document> GetDocumentByIdAsync(int documentId);
        Task<List<Document>> GetAllDocumentsAsync();
        Task AddEmployeeAsync(Document document);
        Task UpdateDocumentAsync(int documentId, Document document);
        Task DeleteDocumentAsync(int documentId);
    }
}
