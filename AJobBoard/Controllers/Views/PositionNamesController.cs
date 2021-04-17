using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AJobBoard.Data;
using Jobtransparency.Models.Entity;
using Microsoft.AspNetCore.Authorization;

namespace Jobtransparency.Controllers.Views
{
    [Authorize(Roles = "Admin")]
    public class PositionNamesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PositionNamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PositionNames
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PositionName.Include(p => p.JobGettingConfig);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PositionNames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positionName = await _context.PositionName
                .Include(p => p.JobGettingConfig)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (positionName == null)
            {
                return NotFound();
            }

            return View(positionName);
        }

        // GET: PositionNames/Create
        public IActionResult Create()
        {
            ViewData["JobGettingConfigId"] = new SelectList(_context.JobGettingConfig, "Id", "Id");
            return View();
        }

        // POST: PositionNames/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,JobGettingConfigId")] PositionName positionName)
        {
            if (ModelState.IsValid)
            {
                _context.Add(positionName);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["JobGettingConfigId"] = new SelectList(_context.JobGettingConfig, "Id", "Id", positionName.JobGettingConfigId);
            return View(positionName);
        }

        // GET: PositionNames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positionName = await _context.PositionName.FindAsync(id);
            if (positionName == null)
            {
                return NotFound();
            }
            ViewData["JobGettingConfigId"] = new SelectList(_context.JobGettingConfig, "Id", "Id", positionName.JobGettingConfigId);
            return View(positionName);
        }

        // POST: PositionNames/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,JobGettingConfigId")] PositionName positionName)
        {
            if (id != positionName.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(positionName);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PositionNameExists(positionName.Id))
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
            ViewData["JobGettingConfigId"] = new SelectList(_context.JobGettingConfig, "Id", "Id", positionName.JobGettingConfigId);
            return View(positionName);
        }

        // GET: PositionNames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positionName = await _context.PositionName
                .Include(p => p.JobGettingConfig)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (positionName == null)
            {
                return NotFound();
            }

            return View(positionName);
        }

        // POST: PositionNames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var positionName = await _context.PositionName.FindAsync(id);
            _context.PositionName.Remove(positionName);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PositionNameExists(int id)
        {
            return _context.PositionName.Any(e => e.Id == id);
        }
    }
}
