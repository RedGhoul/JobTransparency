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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AJobBoard.Controllers.Views
{
    public class HomeController : Controller
    {
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IJobPostingRepository jobPostingRepository, ILogger<HomeController> logger)
        {
            _jobPostingRepository = jobPostingRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Home called");
            ViewBag.TotalJobs = await _jobPostingRepository.GetTotalJobs();

            HomeIndexViewModel homeIndexViewModel = new HomeIndexViewModel();
            homeIndexViewModel.FindModel = new FindModel();

            homeIndexViewModel.jobPostings = await _jobPostingRepository.GetRandomSetOfJobPostings();

            homeIndexViewModel.ImageName = "https://staticassetsforsites.s3-us-west-2.amazonaws.com/tech" + new Random().Next(1, 10) + "-min.jpg";

            homeIndexViewModel.TimeToCache = 10;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
