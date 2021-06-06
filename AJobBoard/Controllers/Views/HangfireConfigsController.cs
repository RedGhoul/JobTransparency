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

namespace Jobtransparency.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HangfireConfigsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HangfireConfigsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HangfireConfigs
        public async Task<IActionResult> Index()
        {
            return View(await _context.HangfireConfigs.ToListAsync());
        }

        // GET: HangfireConfigs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangfireConfig = await _context.HangfireConfigs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hangfireConfig == null)
            {
                return NotFound();
            }

            return View(hangfireConfig);
        }

        // GET: HangfireConfigs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HangfireConfigs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SQLCommandTimeOut,AffinityThreshold,MinKeyPhraseLengthThreshold")] HangfireConfig hangfireConfig)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hangfireConfig);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hangfireConfig);
        }

        // GET: HangfireConfigs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangfireConfig = await _context.HangfireConfigs.FindAsync(id);
            if (hangfireConfig == null)
            {
                return NotFound();
            }
            return View(hangfireConfig);
        }

        // POST: HangfireConfigs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SQLCommandTimeOut,AffinityThreshold,MinKeyPhraseLengthThreshold")] HangfireConfig hangfireConfig)
        {
            if (id != hangfireConfig.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hangfireConfig);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HangfireConfigExists(hangfireConfig.Id))
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
            return View(hangfireConfig);
        }

        // GET: HangfireConfigs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangfireConfig = await _context.HangfireConfigs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hangfireConfig == null)
            {
                return NotFound();
            }

            return View(hangfireConfig);
        }

        // POST: HangfireConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hangfireConfig = await _context.HangfireConfigs.FindAsync(id);
            _context.HangfireConfigs.Remove(hangfireConfig);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HangfireConfigExists(int id)
        {
            return _context.HangfireConfigs.Any(e => e.Id == id);
        }
    }
}
