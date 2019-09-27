using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AJobBoard.Data;
using AJobBoard.Models;
using System.IO;
using System.Text;
using System.Web;

namespace AJobBoard.Controllers
{
    public class JobPostingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobPostingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(HomeIndexViewModel homeIndexVM)
        {
            SetDefaultFindModel(homeIndexVM);
            SetupViewBag(homeIndexVM);
            List<JobPosting> Jobs = await ConfigureSearchAsync(homeIndexVM);
            Jobs = ConfigurePaging(homeIndexVM, Jobs);
            return View(Jobs);
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
            jobPosting = await TickNumberOfViewAsync(jobPosting);

            return View(jobPosting);
        }

        // GET: JobPostings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobPostings/Create
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
        public async Task<JobPosting> TickNumberOfViewAsync(JobPosting jobPosting)
        {
            jobPosting.NumberOfViews++;

            try
            {
                _context.Update(jobPosting);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobPostingExists(jobPosting.Id))
                {
                }
                else
                {
                    throw;
                }
            }
            return jobPosting;
        }

        private async Task<List<JobPosting>> ConfigureSearchAsync(HomeIndexViewModel homeIndexVM)
        {
            IQueryable<JobPosting> jobsQuery = null;
            List<JobPosting> Jobs = null;
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            // find By Location
            if (homeIndexVM.FindModel.Location.ToLower().Equals("anywhere"))
            {
                jobsQuery = _context.JobPostings;
            }
            else if (homeIndexVM.FindModel.Location.ToLower().Equals("ontario"))
            {
                jobsQuery = _context.JobPostings.Where(x => x.Location.Contains("vancouver") == false);
            }
            else
            {
                jobsQuery = _context.JobPostings.Where(x => x.Location.Contains(homeIndexVM.FindModel.Location) == true);
            }

            // find By Key Words

            jobsQuery = jobsQuery.Where(x => x.Title.Contains(homeIndexVM.FindModel.KeyWords) ||
                                        x.Summary.Contains(homeIndexVM.FindModel.KeyWords));

            // add Max Results

            Jobs = await jobsQuery.Take(homeIndexVM.FindModel.MaxResults).ToListAsync();

            // Calculate time
            TimeSpan duration = end - start;

            ViewBag.SecsToQuery = duration.TotalSeconds.ToString().Replace("-", "");

            return Jobs;
        }

        private List<JobPosting> ConfigurePaging(HomeIndexViewModel homeIndexVM, List<JobPosting> Jobs)
        {
            int PageSize = 12;

            var count = Jobs.Count();

            Jobs = Jobs.Skip((int)homeIndexVM.FindModel.Page * PageSize).Take(PageSize).ToList();
            if (PageSize == 0)
            {
                ViewBag.MaxPage = 10;
            }
            else
            {
                ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);
            }

            ViewBag.Page = homeIndexVM.FindModel.Page;
            return Jobs;
        }

        private void SetupViewBag(HomeIndexViewModel homeIndexVM)
        {
            ViewBag.Location = homeIndexVM.FindModel.Location;
            ViewBag.KeyWords = homeIndexVM.FindModel.KeyWords;
            ViewBag.MaxResults = homeIndexVM.FindModel.MaxResults;
            if (homeIndexVM.FindModel.MaxResults != 0)
            {
                ViewBag.TotalJobs = homeIndexVM.FindModel.MaxResults;
            }
            else
            {
                ViewBag.TotalJobs = 100;
            }
        }

        private static void SetDefaultFindModel(HomeIndexViewModel homeIndexVM)
        {
            if (homeIndexVM.FindModel == null)
            {
                homeIndexVM.FindModel = new FindModel();
                homeIndexVM.FindModel.Location = "anywhere";
                homeIndexVM.FindModel.KeyWords = "";
                homeIndexVM.FindModel.MaxResults = 100;
                homeIndexVM.FindModel.Page = 0;
            }
        }
    }
}
