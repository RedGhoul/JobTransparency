using AJobBoard.Data;
using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Models.View;
using AJobBoard.Services;
using AJobBoard.Utils.ControllerHelpers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AJobBoard.Controllers.Views
{
    [AllowAnonymous]
    public class JobPostingsController : Controller
    {
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly INLTKService _nltkService;
        private readonly IKeyPharseRepository _keyPharseRepository;
        private readonly IMapper _mapper;

        public JobPostingsController(
            IMapper mapper,
            IJobPostingRepository jobPostingRepository,
            INLTKService nltkService,
            IKeyPharseRepository keyPharseRepository)
        {
            _jobPostingRepository = jobPostingRepository;
            _nltkService = nltkService;
            _keyPharseRepository = keyPharseRepository;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, string? keywords)
        {

            HomeIndexViewModel homeIndexVm = JobPostingHelper.SetDefaultFindModel(new HomeIndexViewModel());

            homeIndexVm.FindModel.Page = pageNumber ?? 1;
            homeIndexVm.FindModel.KeyWords = keywords ?? "";

            JobPostingHelper.SetupViewBag(homeIndexVm, ViewBag);

            List<JobPostingDTO> result = await _jobPostingRepository.ConfigureSearch(homeIndexVm);

            string count = _jobPostingRepository.GetTotal();

            ViewBag.MaxPage = int.Parse(count) / homeIndexVm.FindModel.Page;

            ViewBag.Page = homeIndexVm.FindModel.Page;
            homeIndexVm.JobPostings = result;
            return View(homeIndexVm);
        }

        [HttpPost]
        public IActionResult IndexPost(HomeIndexViewModel homeIndexVm)
        {
            homeIndexVm = JobPostingHelper.SetDefaultFindModel(homeIndexVm);

            JobPostingHelper.SetupViewBag(homeIndexVm, ViewBag);

            return RedirectToAction("Index", new
            {
                pageNumber = homeIndexVm.FindModel.Page,
                keywords = homeIndexVm.FindModel.KeyWords
            });
        }


        // GET: JobPostings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            JobPosting jobPosting = await _jobPostingRepository.GetById(id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            jobPosting = await _jobPostingRepository.IncrementNumberOfViews(jobPosting);

            return View(new JobPostingDetailViewModel()
            {
                CurrentJobPosting = jobPosting,
            });
        }

    }
}
