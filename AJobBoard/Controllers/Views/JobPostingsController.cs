using AJobBoard.Data;
using AJobBoard.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Controllers
{
    public class JobPostingsController : Controller
    {
        private readonly IJobPostingRepository _JobPostingRepository;

        public JobPostingsController(IJobPostingRepository JobPostingRepository)
        {
            _JobPostingRepository = JobPostingRepository;
        }

        public async Task<IActionResult> Index(HomeIndexViewModel homeIndexVM)
        {
            SetDefaultFindModel(homeIndexVM);
            SetupViewBag(homeIndexVM);
            (List<JobPosting> Jobs, TimeSpan duration) = await _JobPostingRepository.ConfigureSearchAsync(homeIndexVM);
            ViewBag.SecsToQuery = duration.TotalSeconds.ToString().Replace("-", "");
            Jobs = ConfigurePaging(homeIndexVM, Jobs);
            return View(Jobs);
        }


        // GET: JobPostings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            JobPosting jobPosting = await _JobPostingRepository.GetJobPostingById(id);
            if (jobPosting == null)
            {
                return NotFound();
            }
            jobPosting = await _JobPostingRepository.TickNumberOfViewAsync(jobPosting);

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
                await _JobPostingRepository.CreateJobPostingAsync(jobPosting);
                return RedirectToAction(nameof(Index));
            }
            return View(jobPosting);
        }

        // GET: JobPostings/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            JobPosting jobPosting = await _JobPostingRepository.GetJobPostingById(id);
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
                await _JobPostingRepository.PutJobPostingAsync(id, jobPosting);
            }
            return View(jobPosting);
        }

        // GET: JobPostings/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            JobPosting jobPosting = await _JobPostingRepository.GetJobPostingById(id);
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
            await _JobPostingRepository.DeleteJobPostingAsync(id);
            return RedirectToAction(nameof(Index));
        }



        // Helpers

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
