using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AJobBoard.Data;
using AJobBoard.Models;
using Microsoft.AspNetCore.Mvc;

namespace AJobBoard.Controllers.Views
{
    public class JobPostingsController : Controller
    {
        private readonly IJobPostingRepository _jobPostingRepository;

        public JobPostingsController(IJobPostingRepository jobPostingRepository)
        {
            _jobPostingRepository = jobPostingRepository;
        }

        public async Task<IActionResult> Index(HomeIndexViewModel homeIndexVm)
        {
            homeIndexVm = SetDefaultFindModel(homeIndexVm);
            SetupViewBag(homeIndexVm);
            var (jobs, duration) = await _jobPostingRepository.ConfigureSearchAsync(homeIndexVm);
            ViewBag.SecsToQuery = duration.TotalSeconds
                .ToString(CultureInfo.CurrentCulture)
                .Replace("-", "");
            jobs = ConfigurePaging(homeIndexVm, jobs);
            return View(jobs);
        }


        // GET: JobPostings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            JobPosting jobPosting = await _jobPostingRepository.GetJobPostingById(id);
            if (jobPosting == null)
            {
                return NotFound();
            }
            jobPosting = await _jobPostingRepository.TickNumberOfViewAsync(jobPosting);

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
                await _jobPostingRepository.CreateJobPostingAsync(jobPosting);
                return RedirectToAction(nameof(Index));
            }
            return View(jobPosting);
        }

        // GET: JobPostings/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            JobPosting jobPosting = await _jobPostingRepository.GetJobPostingById(id);
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
                await _jobPostingRepository.PutJobPostingAsync(id, jobPosting);
            }
            return View(jobPosting);
        }

        // GET: JobPostings/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            JobPosting jobPosting = await _jobPostingRepository.GetJobPostingById(id);
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
            await _jobPostingRepository.DeleteJobPostingAsync(id);
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
            ViewBag.TotalJobs = homeIndexVM.FindModel.MaxResults != 0 ? homeIndexVM.FindModel.MaxResults : 100;
        }

        private static HomeIndexViewModel SetDefaultFindModel(HomeIndexViewModel homeIndexVM)
        {
            if (homeIndexVM == null)
            {
                homeIndexVM = new HomeIndexViewModel
                {
                    FindModel = new FindModel
                    {
                        Location = "anywhere",
                        KeyWords = "",
                        MaxResults = 100,
                        Page = 0
                    }
                };
            }
            else if (homeIndexVM.FindModel == null)
            {
                homeIndexVM.FindModel = new FindModel();

                homeIndexVM = FillFindModel(homeIndexVM);
            }
            else
            {
                homeIndexVM = FillFindModel(homeIndexVM);
            }
            return homeIndexVM;
        }

        private static HomeIndexViewModel FillFindModel(HomeIndexViewModel homeIndexVM)
        {
            homeIndexVM.FindModel.KeyWords = homeIndexVM.FindModel.KeyWords ?? "";
            homeIndexVM.FindModel.Location = homeIndexVM.FindModel.Location ?? "";
            if (homeIndexVM.FindModel.MaxResults == 0)
            {
                homeIndexVM.FindModel.MaxResults = 100;
            }

            if (homeIndexVM.FindModel.Page == 0)
            {
                homeIndexVM.FindModel.Page = 1;
            }

            return homeIndexVM;
        }


    }
}
