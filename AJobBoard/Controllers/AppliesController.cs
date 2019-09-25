using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AJobBoard.Data;
using AJobBoard.Models;
using Microsoft.AspNetCore.Identity;

namespace AJobBoard.Controllers
{
    public class AppliesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppliesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Applies
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apply = await _context.Applies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (apply == null)
            {
                return NotFound();
            }
            return View(apply);
        }

        //// POST: Applies/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id")] Apply apply)
        //{
        //    var currentUser = await _userManager.GetUserAsync(HttpContext.User);

        //    var job = _context.JobPostings.Where(x => x.Id == apply.Id).FirstOrDefault();
        //    if(job != null)
        //    {
        //        if(currentUser.Applies != null)
        //        {
        //            currentUser.Applies.Add(new Apply
        //            {
        //                DateAddedToApplies = DateTime.Now,
        //                JobPosting = job
        //            });
        //        } else
        //        {
        //            currentUser.Applies = new List<Apply>();
        //            currentUser.Applies.Add(new Apply
        //            {
        //                DateAddedToApplies = DateTime.Now,
        //                JobPosting = job
        //            });
        //        }

        //        await _context.SaveChangesAsync();
        //    }

        //    return Ok();
        //}

        //// GET: Applies/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var apply = await _context.Applies.FindAsync(id);
        //    if (apply == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(apply);
        //}

        //// POST: Applies/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,DateAddedToApplies")] Apply apply)
        //{
        //    if (id != apply.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(apply);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ApplyExists(apply.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(apply);
        //}

        //// GET: Applies/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var apply = await _context.Applies
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (apply == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(apply);
        //}

        //// POST: Applies/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var apply = await _context.Applies.FindAsync(id);
        //    _context.Applies.Remove(apply);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ApplyExists(int id)
        //{
        //    return _context.Applies.Any(e => e.Id == id);
        //}
    }
}
