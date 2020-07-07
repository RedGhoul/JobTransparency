using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AJobBoard.Controllers.Views
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IDocumentRepository _documentRepository;
        public DocumentsController(
            IUserRepository userRepository,
            IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
            _userRepository = userRepository;
        }

        // GET: Documents
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userRepository
                .getUserFromHttpContextAsync(HttpContext);
            List<Document> documents = _documentRepository
                .GetDocumentsOfCurrentUser(currentUser.Id);

            return View(documents);
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _documentRepository.GetDocumentByIdAsync(id);

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
            var currentUser = await _userRepository
                .getUserFromHttpContextAsync(HttpContext);


            var saveResult = await _documentRepository
                .SaveDocumentToUser(document, currentUser);

            if (saveResult)
            {
                return RedirectToAction(nameof(Index));
            }


            return View(document);
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _documentRepository.GetDocumentByIdAsync(id);
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
                await _documentRepository.UpdateDocument(document);

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

            var document = await _documentRepository.GetDocumentByIdAsync(id);

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
            var User = await _userRepository
                .getUserFromHttpContextAsync(HttpContext);

            await _documentRepository.RemoveDocumentFromUser(id, User);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Download(int id, string fileName)
        {
            var currentUser = await _userRepository
                .getUserFromHttpContextAsync(HttpContext);
            var file = await _documentRepository.DownLoadDocument(id, currentUser);

            if (file != null)
            {
                var contentType = "APPLICATION/octet-stream";
                fileName = fileName + ".pdf";
                return File(file, contentType, fileName);
            }

            return RedirectToAction("Details", new { id = id });

        }






    }
}
