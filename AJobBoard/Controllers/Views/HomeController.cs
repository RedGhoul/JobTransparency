using AJobBoard.Data;
using AJobBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalJobs = await _context.JobPostings.CountAsync();

            HomeIndexViewModel homeIndexViewModel = new HomeIndexViewModel();
            homeIndexViewModel.FindModel = new FindModel();
            List<JobPosting> Jobs = await GetRandomSetOfJobPostings();

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
