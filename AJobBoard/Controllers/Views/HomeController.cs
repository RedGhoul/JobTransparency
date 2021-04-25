using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.View;
using AJobBoard.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Controllers.Views
{
    public class HomeController : Controller
    {
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IKeyPharseRepository _keyPharseRepository;
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _ctx;

        public HomeController(ApplicationDbContext ctx, IMapper mapper, IKeyPharseRepository keyPharseRepository, 
            IJobPostingRepository jobPostingRepository, ILogger<HomeController> logger)
        {
            _jobPostingRepository = jobPostingRepository;
            _logger = logger;
            _keyPharseRepository = keyPharseRepository;
            _mapper = mapper;
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalJobs = _jobPostingRepository.GetTotal();
            ViewBag.TotalCompanies = _ctx.JobPostings.Select(x => x.Company).Distinct().Count();
            return View(
                new HomeIndexViewModel(
                    await _jobPostingRepository.GetRandomSet(),
                    new FindModel(),
                    10)
                );
        }

        public IActionResult About()
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
