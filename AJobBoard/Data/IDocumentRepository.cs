using System.Collections.Generic;
using System.Threading.Tasks;
using AJobBoard.Models;
using AJobBoard.Models.View;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AJobBoard.Data
{
    public interface IDocumentRepository
    {
        bool DocumentExists(int id);
        Task<Document> GetDocumentByIdAsync(int? id);
        List<Document> GetDocumentsOfCurrentUser(string userId);
        void RemoveDocumentFromUser(int documentId, ApplicationUser user);
        Task<bool> SaveDocumentToUser(DocumentViewModel document, ApplicationUser user);
        Task<bool> UpdateDocument(Document document);
    }
}