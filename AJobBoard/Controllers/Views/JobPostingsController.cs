using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.Data;
using AJobBoard.Models.View;
using AJobBoard.Services;
using AJobBoard.Utils.ControllerHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        private readonly string _searchVmCacheKey = "SearchVMCacheKey";
        public JobPostingsController(
            IJobPostingRepository jobPostingRepository,
            INLTKService nltkService,
            IKeyPharseRepository keyPharseRepository)
        {
            _jobPostingRepository = jobPostingRepository;
            _nltkService = nltkService;
            _keyPharseRepository = keyPharseRepository;
        }


        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, string? keywords)
        {
            HomeIndexViewModel homeIndexVm = JobPostingHelper.SetDefaultFindModel(new HomeIndexViewModel());

            homeIndexVm.FindModel.Page = pageNumber ?? 1;
            homeIndexVm.FindModel.KeyWords = keywords ?? "";

            JobPostingHelper.SetupViewBag(homeIndexVm, ViewBag);

            List<Models.DTO.JobPostingDTO> result = await _jobPostingRepository.ConfigureSearch(homeIndexVm);

            string count =  _jobPostingRepository.GetTotal();

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

            return RedirectToAction("Index",new { pageNumber = homeIndexVm.FindModel.Page ,
                keywords = homeIndexVm.FindModel.KeyWords});
        }


        // GET: JobPostings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            JobPosting jobPosting = await _jobPostingRepository.GetById(id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            jobPosting = await _jobPostingRepository.AddView(jobPosting);

            return View(jobPosting);
        }


        // GET: JobPostings/Create
        [Authorize(Policy = "CanCreatePosting")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobPostings/Create
        [HttpPost]
        [Authorize(Policy = "CanCreatePosting")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,URL,Company,Location,PostDate,Salary,Posters")] JobPosting jobPosting)
        {
            if (ModelState.IsValid)
            {
                JobPosting newPosting = await _jobPostingRepository.Create(jobPosting);

                Models.DTO.KeyPhrasesWrapperDTO wrapper = await _nltkService.GetNLTKKeyPhrases(jobPosting.Description);

                if (newPosting.KeyPhrases == null)
                {
                    newPosting.KeyPhrases = new List<KeyPhrase>();
                }

                foreach (Models.DTO.KeyPhraseDTO item in wrapper.rank_list)
                {
                    newPosting.KeyPhrases.Add(new KeyPhrase
                    {
                        Affinty = item.Affinty,
                        Text = item.Text
                    });
                }

                Models.DTO.SummaryDTO nltkSummary = await _nltkService.GetNLTKSummary(jobPosting.Description);

                newPosting.Summary = nltkSummary.SummaryText;

                await _jobPostingRepository.Put(newPosting.Id, newPosting);

                return RedirectToAction(nameof(Index));
            }
            return View(jobPosting);
        }

        // GET: JobPostings/Edit/5
        [Authorize(Policy = "CanEditPosting")]
        public async Task<IActionResult> Edit(int id)
        {
            JobPosting jobPosting = await _jobPostingRepository.GetById(id);
            if (jobPosting == null)
            {
                return NotFound();
            }
            return View(jobPosting);
        }

        // POST: JobPostings/Edit/5
        [HttpPost]
        [Authorize(Policy = "CanEditPosting")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Summary,URL,Company,Location,PostDate,Salary,Posters")] JobPosting jobPosting)
        {
            if (id != jobPosting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _jobPostingRepository.Put(id, jobPosting);
            }
            return View(jobPosting);
        }


        // GET: JobPostings/Delete/5
        [Authorize(Policy = "CanDeletePosting")]
        public async Task<IActionResult> Delete(int id)
        {
            JobPosting jobPosting = await _jobPostingRepository.GetById(id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            return View(jobPosting);
        }

        // POST: JobPostings/Delete/5
        [Authorize(Policy = "CanDeletePosting")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _jobPostingRepository.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
