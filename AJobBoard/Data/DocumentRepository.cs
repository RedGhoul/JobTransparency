using AJobBoard.Models;
using AJobBoard.Models.View;
using AJobBoard.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Data
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IAWSService _AWSService;
        public DocumentRepository(ApplicationDbContext ctx, IAWSService AWSService)
        {
            _ctx = ctx;
            _AWSService = AWSService;
        }

        public async Task<Document> GetDocumentByIdAsync(int? id)
        {
            var document = await _ctx.Document
                .FirstOrDefaultAsync(m => m.DocumentId == id);

            return document;
        }

        public async Task<bool> SaveDocumentToUser(DocumentViewModel document, ModelStateDictionary modelState, ApplicationUser user)
        {

            _AWSService.validateFile(document.Resume, modelState);

            string resumeKEY = "";
            if (user != null)
            {
                resumeKEY = await _AWSService.UploadStreamToBucket("ajobboard",
                    "Resumes/" + user.Id + document.Resume.FileName.Replace(" ", "").Replace("-", ""),
                    document.Resume.ContentType, document.Resume.OpenReadStream());

                var tempDoc = new Document()
                {
                    DocumentName = document.DocumentName,
                    DateCreated = DateTime.Now,
                    IsOtherDoc = document.IsOtherDoc,
                    IsResume = document.IsResume,
                    URL = resumeKEY
                };

                _ctx.Document.Add(tempDoc);
                if (user.Documents == null)
                {
                    user.Documents = new List<Document>()
                    {
                        tempDoc
                    };
                }
                else
                {
                    user.Documents.Add(tempDoc);
                }


                await _ctx.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async void RemoveDocumentFromUser(int documentId, ApplicationUser user)
        {

            var document = await _ctx.Document.FindAsync(documentId);

            user.Documents.Remove(document);
            await _AWSService.DeleteFile(document.URL, "ajobboard");
            _ctx.Document.Remove(document);
            await _ctx.SaveChangesAsync();
        }

        public async Task<bool> UpdateDocument(Document document)
        {
            try
            {
                _ctx.Update(document);
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentExists(document.DocumentId))
                {
                    return false;
                }
                else
                {
                    // Need to log this out some where
                    return false;
                }
            }
            return true;
        }

        public bool DocumentExists(int id)
        {
            return _ctx.Document.Any(e => e.DocumentId == id);
        }

        public List<Document> GetDocumentsOfCurrentUser(string userId)
        {
            //var User = await _userManager.GetUserAsync(HttpContext.User);
            var Documents = _ctx.Users.Include(x => x.Documents)
                .Where(x => x.Id.Equals(userId)).Select(x => x.Documents).FirstOrDefault();
            return Documents;
        }

    }
}
