using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AJobBoard.Models;
using AJobBoard.Data;
using Microsoft.EntityFrameworkCore;

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
            homeIndexViewModel.FindModel = new FindModel
            {

            };
            homeIndexViewModel.jobPostings = await _context.JobPostings.Take(10).ToListAsync();

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
    }
}
