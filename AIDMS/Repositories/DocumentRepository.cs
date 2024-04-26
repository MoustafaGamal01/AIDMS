using AIDMS.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Metadata;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AIDMS.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AIDMSContextClass context;

        public DocumentRepository(AIDMSContextClass context) {
            this.context = context;
        }

        public async Task AddDocumentAsync(AIDocument document)
        {
            context.Documents.Add(document);
            await context.SaveChangesAsync();
        }

        public async Task DeleteDocumentAsync(int documentId)
        {
            AIDocument document = await GetDocumentByIdAsync(documentId);
            if (document == null)
            {
                throw new InvalidOperationException($"Document with ID {documentId} not found.");
            }
            context.Documents.Remove(document);
            await context.SaveChangesAsync();
        }

        public async Task<List<AIDocument>> GetAllDocumentsAsync()
        {
            return await context.Documents.ToListAsync();
        }

        public async Task<AIDMS.Entities.AIDocument> GetDocumentByIdAsync(int documentId)
        {
            return await context.Documents.FirstOrDefaultAsync(d => d.Id == documentId);
        }

        public async Task UpdateDocumentAsync(int documentId, AIDMS.Entities.AIDocument document)
        {
            AIDocument existingDocument = await GetDocumentByIdAsync(documentId);
            if (existingDocument == null)
            {
                throw new InvalidOperationException($"Document with ID {documentId} not found.");
            }
            context.Entry(existingDocument).CurrentValues.SetValues(document);
            await context.SaveChangesAsync();
        }
    }
}
