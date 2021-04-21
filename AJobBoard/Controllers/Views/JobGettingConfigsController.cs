using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AJobBoard.Data;
using Jobtransparency.Models.Entity;

namespace Jobtransparency.Controllers.Views
{
    public class JobGettingConfigsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobGettingConfigsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: JobGettingConfigs
        public async Task<IActionResult> Index()
        {
            return View(await _context.JobGettingConfig.ToListAsync());
        }

        // GET: JobGettingConfigs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobGettingConfig = await _context.JobGettingConfig
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobGettingConfig == null)
            {
                return NotFound();
            }

            return View(jobGettingConfig);
        }

        // GET: JobGettingConfigs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobGettingConfigs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaxAge,MaxNumber,Host,LinkCheckIfJobExists,LinkAzureFunction,LinkAzureFunction2,LinkJobPostingCreation")] JobGettingConfig jobGettingConfig)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobGettingConfig);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jobGettingConfig);
        }

        // GET: JobGettingConfigs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobGettingConfig = await _context.JobGettingConfig.FindAsync(id);
            if (jobGettingConfig == null)
            {
                return NotFound();
            }
            return View(jobGettingConfig);
        }

        // POST: JobGettingConfigs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaxAge,MaxNumber,Host,LinkCheckIfJobExists,LinkAzureFunction,LinkAzureFunction2,LinkJobPostingCreation")] JobGettingConfig jobGettingConfig)
        {
            if (id != jobGettingConfig.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobGettingConfig);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobGettingConfigExists(jobGettingConfig.Id))
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
            return View(jobGettingConfig);
        }

        // GET: JobGettingConfigs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobGettingConfig = await _context.JobGettingConfig
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobGettingConfig == null)
            {
                return NotFound();
            }

            return View(jobGettingConfig);
        }

        // POST: JobGettingConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobGettingConfig = await _context.JobGettingConfig.FindAsync(id);
            _context.JobGettingConfig.Remove(jobGettingConfig);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobGettingConfigExists(int id)
        {
            return _context.JobGettingConfig.Any(e => e.Id == id);
        }
    }
}
