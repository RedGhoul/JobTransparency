using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.View;
using AJobBoard.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AJobBoard.Controllers.Views
{
    public class DocumentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AWSService _AWSService;

        public DocumentsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AWSService AWSService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _AWSService = AWSService;
        }

        // GET: Documents
        public async Task<IActionResult> Index()
        {
            List<Document> Documents = await GetDocumentsOfCurrentUser();

            return View(Documents);
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Document
                .FirstOrDefaultAsync(m => m.DocumentId == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // GET: Documents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Documents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentViewModel document)
        {
            var User = await _userManager.GetUserAsync(HttpContext.User);

            _AWSService.validateFile(document.Resume, ModelState);

            string resumeKEY = "";
            if (User != null)
            {
                resumeKEY = await _AWSService.UploadStreamToBucket("ajobboard",
                    "Resumes/" + User.Id + document.Resume.FileName.Replace(" ","").Replace("-",""),
                    document.Resume.ContentType, document.Resume.OpenReadStream());

                var tempDoc = new Document()
                {
                    DocumentName = document.DocumentName,
                    DateCreated = DateTime.Now,
                    IsOtherDoc = document.IsOtherDoc,
                    IsResume = document.IsResume,
                    URL = resumeKEY
                };

                _context.Document.Add(tempDoc);
                if (User.Documents == null)
                {
                    User.Documents = new List<Document>()
                    {
                        tempDoc
                    };
                }
                else
                {
                    User.Documents.Add(tempDoc);
                }
                

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(document);
        }

        public async Task<FileResult> Download(string fileName)
        {
            byte[] fileBytes = await _AWSService.GetFileInBytes(fileName, "ajobboard");
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Document.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            return View(document);
        }

        // POST: Documents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DocumentId,URL,IsResume,IsOtherDoc,DateCreated")] Document document)
        {
            if (id != document.DocumentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(document);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.DocumentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(document);
        }


        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Document
                .FirstOrDefaultAsync(m => m.DocumentId == id);

            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var User = await _userManager.GetUserAsync(HttpContext.User);
            var document = await _context.Document.FindAsync(id);

            User.Documents.Remove(document);
            await _AWSService.DeleteFile(document.URL, "ajobboard");
            _context.Document.Remove(document);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id)
        {
            return _context.Document.Any(e => e.DocumentId == id);
        }

        private async Task<List<Document>> GetDocumentsOfCurrentUser()
        {
            var User = await _userManager.GetUserAsync(HttpContext.User);
            var Documents = _context.Users.Include(x => x.Documents)
                .Where(x => x.Id.Equals(User.Id)).Select(x => x.Documents).FirstOrDefault();
            return Documents;
        }
    }
}
