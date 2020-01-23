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
            List<Document> Documents = _documentRepository
                .GetDocumentsOfCurrentUser(currentUser.Id);

            return View(Documents);
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
                .SaveDocumentToUser(document, ModelState, currentUser);

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

            _documentRepository.RemoveDocumentFromUser(id, User);
           
            return RedirectToAction(nameof(Index));
        }



       
    }
}
