using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AJobBoard.Data;
using AJobBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace AJobBoard.Controllers.Views
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IDistributedCache _cache;

        public HomeController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            string TotalJobs = await _cache.GetStringAsync("TotalJobs");

            if (string.IsNullOrEmpty(TotalJobs))
            {
                int TotalJobsI = 0;
                ViewBag.TotalJobs = await _context.JobPostings.CountAsync();
                TotalJobsI = ViewBag.TotalJobs;
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromMinutes(40));
                await _cache.SetStringAsync("TotalJobs", TotalJobsI.ToString(), options);
            }
            else
            {
                ViewBag.TotalJobs = TotalJobs;
            }

            string Randomjobs = _cache.GetString("RandomJobsFrontPage");
            List<JobPosting> RandomjobsList = null;
            if (string.IsNullOrEmpty(Randomjobs))
            {
                RandomjobsList = await GetRandomSetOfJobPostings();
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromMinutes(40));
                await _cache.SetStringAsync("RandomJobsFrontPage", JsonConvert.SerializeObject(RandomjobsList), options);
            }
            else
            {
                RandomjobsList = JsonConvert.DeserializeObject<List<JobPosting>>(Randomjobs);
            }

            HomeIndexViewModel homeIndexViewModel = new HomeIndexViewModel();
            homeIndexViewModel.FindModel = new FindModel();
            List<JobPosting> Jobs = RandomjobsList;

            homeIndexViewModel.jobPostings = Jobs;

            return View(homeIndexViewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult RegisterType()
        {
            return View();
        }
        public IActionResult JobSingle()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<List<JobPosting>> GetRandomSetOfJobPostings()
        {
            Random random = new Random();
            int NumberOfResults = random.Next(10, 20);
            var Jobs = await _context.JobPostings.Take(NumberOfResults * 2).ToListAsync();
            int SkipNumberOfResults = random.Next(10, 20);
            Jobs = Jobs.Skip(SkipNumberOfResults).Reverse().ToList();
            return Jobs;
        }
    }
}
