using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AJobBoard.Data;
using Jobtransparency.Models.Entity.JobGetter;
using Microsoft.AspNetCore.Authorization;

namespace Jobtransparency.Controllers.Views
{
    [Authorize(Roles = "Admin")]
    public class PositionCitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PositionCitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PositionCities
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PositionCities.Include(p => p.JobGettingConfig);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PositionCities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positionCities = await _context.PositionCities
                .Include(p => p.JobGettingConfig)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (positionCities == null)
            {
                return NotFound();
            }

            return View(positionCities);
        }

        // GET: PositionCities/Create
        public IActionResult Create()
        {
            ViewData["JobGettingConfigId"] = new SelectList(_context.JobGettingConfig, "Id", "Id");
            return View();
        }

        // POST: PositionCities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,JobGettingConfigId")] PositionCities positionCities)
        {
            if (ModelState.IsValid)
            {
                _context.Add(positionCities);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["JobGettingConfigId"] = new SelectList(_context.JobGettingConfig, "Id", "Id", positionCities.JobGettingConfigId);
            return View(positionCities);
        }

        // GET: PositionCities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positionCities = await _context.PositionCities.FindAsync(id);
            if (positionCities == null)
            {
                return NotFound();
            }
            ViewData["JobGettingConfigId"] = new SelectList(_context.JobGettingConfig, "Id", "Id", positionCities.JobGettingConfigId);
            return View(positionCities);
        }

        // POST: PositionCities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,JobGettingConfigId")] PositionCities positionCities)
        {
            if (id != positionCities.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(positionCities);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PositionCitiesExists(positionCities.Id))
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
            ViewData["JobGettingConfigId"] = new SelectList(_context.JobGettingConfig, "Id", "Id", positionCities.JobGettingConfigId);
            return View(positionCities);
        }

        // GET: PositionCities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positionCities = await _context.PositionCities
                .Include(p => p.JobGettingConfig)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (positionCities == null)
            {
                return NotFound();
            }

            return View(positionCities);
        }

        // POST: PositionCities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var positionCities = await _context.PositionCities.FindAsync(id);
            _context.PositionCities.Remove(positionCities);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PositionCitiesExists(int id)
        {
            return _context.PositionCities.Any(e => e.Id == id);
        }
    }
}
