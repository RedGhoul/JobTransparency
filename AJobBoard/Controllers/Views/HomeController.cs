using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.View;
using AJobBoard.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AJobBoard.Controllers.Views
{
    public class HomeController : Controller
    {
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IKeyPharseRepository _keyPharseRepository;
        private readonly ILogger<HomeController> _logger;
        private readonly ElasticService _es;
        private readonly IMapper _mapper;
        public HomeController(IMapper mapper, IKeyPharseRepository keyPharseRepository, IJobPostingRepository jobPostingRepository, ILogger<HomeController> logger, ElasticService elasticService)
        {
            _jobPostingRepository = jobPostingRepository;
            _logger = logger;
            _es = elasticService;
            _keyPharseRepository = keyPharseRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Home called");

            ViewBag.TotalJobs = await _jobPostingRepository.GetTotalJobs();
            return View(
                new HomeIndexViewModel(
                    await _jobPostingRepository.GetRandomSetOfJobPostings(),
                    new FindModel(),
                    10)
                );
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult DataIngest()
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
