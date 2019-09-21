using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AJobBoard.Data;
using AJobBoard.Models;

namespace AJobBoard.Controllers
{
    public class JobPostingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobPostingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string Location, string KeyWords, int? MaxResults)
        {
            ViewBag.TotalJobs = await _context.JobPostings.CountAsync();
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            List<JobPosting> Jobs = null;
            if (MaxResults == 0 || Location == null)
            {
                Jobs = await _context.JobPostings.OrderByDescending(j => j.Title)
                    .Where(x => x.Location.Contains(Location ?? "") &&
                           x.Summary.Contains(KeyWords ?? "") &&
                           x.Title.Contains(KeyWords ?? ""))
                    .Take(50)
                    .Distinct()
                    .ToListAsync();
                start = DateTime.Now;
            }
            else
            {
                if (Location != null && Location.ToLower().Equals("anywhere"))
                {
                    Jobs = await _context.JobPostings.OrderByDescending(j => j.Title)
                        .Where(x => x.Summary.Contains(KeyWords ?? "") ||
                               x.Title.Contains(KeyWords ?? ""))
                        .Take((int)MaxResults)
                        .Distinct()
                        .ToListAsync();
                    start = DateTime.Now;
                }
                else
                {
                    Jobs = await _context.JobPostings.OrderByDescending(j => j.Title)
                        .Where(x => x.Location.Contains(Location ?? "") ||
                               x.Summary.Contains(KeyWords ?? "") ||
                               x.Title.Contains(KeyWords ?? ""))
                        .Take((int)MaxResults)
                        .Distinct()
                        .ToListAsync();
                    start = DateTime.Now;
                }

            }
            TimeSpan duration = end - start;
            ViewBag.SecsToQuery = duration.TotalSeconds.ToString().Replace("-","");
            return View(Jobs);
        }

        [HttpPost]
        public IActionResult Find(HomeIndexViewModel homeIndexVM)
        {
            return RedirectToAction("Index", new
            {
                Location = homeIndexVM.FindModel.Location,
                KeyWords = homeIndexVM.FindModel.KeyWords,
                MaxResults = homeIndexVM.FindModel.MaxResults
            });
        }

        // GET: JobPostings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPosting = await _context.JobPostings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            return View(jobPosting);
        }

        // GET: JobPostings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobPostings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Summary,URL,Company,Location,PostDate,Salary,Posters")] JobPosting jobPosting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobPosting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jobPosting);
        }

        // GET: JobPostings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPosting = await _context.JobPostings.FindAsync(id);
            if (jobPosting == null)
            {
                return NotFound();
            }
            return View(jobPosting);
        }

        // POST: JobPostings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Summary,URL,Company,Location,PostDate,Salary,Posters")] JobPosting jobPosting)
        {
            if (id != jobPosting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobPosting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobPostingExists(jobPosting.Id))
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
            return View(jobPosting);
        }

        // GET: JobPostings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPosting = await _context.JobPostings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            return View(jobPosting);
        }

        // POST: JobPostings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);
            _context.JobPostings.Remove(jobPosting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobPostingExists(int id)
        {
            return _context.JobPostings.Any(e => e.Id == id);
        }
    }
}
