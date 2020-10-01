using AJobBoard.Models.Entity;
using AJobBoard.Models.View;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AJobBoard.Data
{
    public interface IDocumentRepository
    {
        bool DocumentExists(int id);
        Task<Document> GetDocumentByIdAsync(int? id);
        Task<MemoryStream> DownLoadDocument(int documentId, ApplicationUser user);
        Task<List<Document>> GetDocumentsOfCurrentUserAsync(string userId);
        Task RemoveDocumentFromUser(int documentId, ApplicationUser user);
        Task<bool> SaveDocumentToUser(DocumentViewModel document, ApplicationUser user);
        Task<bool> UpdateDocument(Document document);
    }
}