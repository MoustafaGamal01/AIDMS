﻿using AIDMS.Entities;
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

        public async Task<List<AIDocument>> GetAllDocumentsByStudentIdAsync(int studentId)
        {
            return await context.Documents.Where(d => d.StudentId == studentId).ToListAsync(); 
        }

        public async Task<List<AIDocument>> GetAllDocumentsByStudentNameAsync(string studentName)
        {
            var std = await context.Students
                .SingleOrDefaultAsync(s => ((s.firstName.Contains(studentName)) ||
                (s.lastName.Contains(studentName)) ||
                (s.firstName + " " + s.lastName).Contains(studentName)));

            if (std == null)
            {
                return new List<AIDocument>();
            }

            var allDocuments = await context.Documents
                .Where(a => a.StudentId == std.Id)
                .ToListAsync();

            return allDocuments;
        }

        public async Task<List<AIDocument>> GetAllDocumentsAsync()
        {
            return await context.Documents.ToListAsync();
        }
        
        public async Task<List<AIDocument>> GetAllDocumentsWithStudentAsync()
        {
            return await context.Documents.Include(doc=>doc.Student).ToListAsync();
        }
        
        public async Task<List<AIDocument>> GetDocumentsByAppIdAndAsync(int appId)
        {
            return await context.Documents
                .Where(doc=>doc.ApplicationId==appId).ToListAsync();
        }
        
        
        public async Task<AIDocument> GetDocumentByIdAsync(int documentId)
        {
            return await context.Documents.FirstOrDefaultAsync(d => d.Id == documentId);
        }

        public async Task UpdateDocumentAsync(int documentId,AIDocument document)
        {
            AIDocument existingDocument = await GetDocumentByIdAsync(documentId);
            if (existingDocument == null)
            {
                throw new InvalidOperationException($"Document with ID {documentId} not found.");
            }
            context.Entry(existingDocument).CurrentValues.SetValues(document);
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
        
        public async Task<List<AIDocument>> GetDocumentsByDocumentTypeAndApplicationTypeAsync(string documentType, string applicationType)
        {

            if (applicationType == "any" && documentType == "any")
            {
                return await context.Documents.ToListAsync();   
            }
            else if (applicationType == "any")
            {
                return await context.Documents
                    .Include(d => d.Application)
                    .Where(d => d.FileType == documentType)
                    .ToListAsync();
            }
            else if (documentType == "any")
            {
                return await context.Documents
                    .Include(d => d.Application)
                    .Where(d => d.Application.Title == applicationType)
                    .ToListAsync();
            }
            else
            {
                return await context.Documents
                    .Include(d => d.Application)
                    .Where(d => d.FileType == documentType && d.Application.Title == applicationType)
                    .ToListAsync();
            }
        }
    }
}
